using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ProductCatalog.CategoriesApi.Models;

namespace ProductCatalog.CategoriesApi.Controllers;

/// <summary>
/// CRUD-микросервис для справочника категорий изделий.
/// Вся работа с PostgreSQL выполнена прямо в контроллере.
/// </summary>
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);
    private static bool _initialized;

    private readonly NpgsqlDataSource _dataSource;

    public CategoriesController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Получить список всех категорий.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        await EnsureDatabaseAsync();

        var categories = new List<Category>();

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name FROM Categories ORDER BY Name;";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categories.Add(new Category
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return Ok(categories);
    }

    /// <summary>
    /// Получить категорию по названию.
    /// </summary>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Category>> GetByName(string name)
    {
        name = Normalize(name);

        if (name.Length == 0)
        {
            return BadRequest("Название категории не может быть пустым.");
        }

        await EnsureDatabaseAsync();

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name FROM Categories WHERE Name = @name;";
        command.Parameters.AddWithValue("name", name);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound($"Категория «{name}» не найдена.");
        }

        return Ok(new Category
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1)
        });
    }

    /// <summary>
    /// Добавить новую категорию.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Category>> Create([FromBody] CategoryRequest request)
    {
        string name = Normalize(request?.Name);

        if (name.Length == 0)
        {
            return BadRequest("Название категории не может быть пустым.");
        }

        await EnsureDatabaseAsync();

        await using var connection = await _dataSource.OpenConnectionAsync();

        if (await CategoryExistsAsync(connection, name))
        {
            return Conflict($"Категория «{name}» уже существует.");
        }

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO Categories (Name)
            VALUES (@name)
            RETURNING Id;
            """;
        command.Parameters.AddWithValue("name", name);

        long id = Convert.ToInt64(await command.ExecuteScalarAsync());
        var category = new Category { Id = id, Name = name };

        return CreatedAtAction(nameof(GetByName), new { name }, category);
    }

    /// <summary>
    /// Изменить название категории. В URL передаётся текущее название,
    /// в теле запроса — новое.
    /// </summary>
    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        string name,
        [FromBody] CategoryRequest request)
    {
        string oldName = Normalize(name);
        string newName = Normalize(request?.Name);

        if (oldName.Length == 0 || newName.Length == 0)
        {
            return BadRequest("Название категории не может быть пустым.");
        }

        await EnsureDatabaseAsync();

        await using var connection = await _dataSource.OpenConnectionAsync();

        if (!await CategoryExistsAsync(connection, oldName))
        {
            return NotFound($"Категория «{oldName}» не найдена.");
        }

        if (!string.Equals(oldName, newName, StringComparison.Ordinal)
            && await CategoryExistsAsync(connection, newName))
        {
            return Conflict($"Категория «{newName}» уже существует.");
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Categories SET Name = @newName WHERE Name = @oldName;";
        command.Parameters.AddWithValue("newName", newName);
        command.Parameters.AddWithValue("oldName", oldName);
        await command.ExecuteNonQueryAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить категорию по названию.
    /// </summary>
    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string name)
    {
        name = Normalize(name);

        if (name.Length == 0)
        {
            return BadRequest("Название категории не может быть пустым.");
        }

        await EnsureDatabaseAsync();

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Categories WHERE Name = @name;";
        command.Parameters.AddWithValue("name", name);

        if (await command.ExecuteNonQueryAsync() == 0)
        {
            return NotFound($"Категория «{name}» не найдена.");
        }

        return NoContent();
    }

    private async Task EnsureDatabaseAsync()
    {
        if (_initialized)
        {
            return;
        }

        await InitializationLock.WaitAsync();
        try
        {
            if (_initialized)
            {
                return;
            }

            const int maxAttempts = 30;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    await using var connection = await _dataSource.OpenConnectionAsync();

                    await using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            """
                            CREATE TABLE IF NOT EXISTS Categories (
                                Id SERIAL PRIMARY KEY,
                                Name TEXT NOT NULL UNIQUE
                            );
                            """;
                        await command.ExecuteNonQueryAsync();
                    }

                    await using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            """
                            INSERT INTO Categories (Name)
                            VALUES
                                (@c1),
                                (@c2),
                                (@c3),
                                (@c4)
                            ON CONFLICT (Name) DO NOTHING;
                            """;
                        command.Parameters.AddWithValue("c1", "Молочные продукты");
                        command.Parameters.AddWithValue("c2", "Хлебобулочные изделия");
                        command.Parameters.AddWithValue("c3", "Напитки");
                        command.Parameters.AddWithValue("c4", "Бакалея");
                        await command.ExecuteNonQueryAsync();
                    }

                    _initialized = true;
                    return;
                }
                catch (NpgsqlException) when (attempt < maxAttempts)
                {
                    await Task.Delay(1000);
                }
            }

            throw new InvalidOperationException(
                "Не удалось подключиться к PostgreSQL и создать таблицу категорий.");
        }
        finally
        {
            InitializationLock.Release();
        }
    }

    private static async Task<bool> CategoryExistsAsync(
        NpgsqlConnection connection,
        string name)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Categories WHERE Name = @name;";
        command.Parameters.AddWithValue("name", name);

        return Convert.ToInt64(await command.ExecuteScalarAsync()) > 0;
    }

    private static string Normalize(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }
}
