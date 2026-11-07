using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using ProductCatalog.CategoriesApi.Models;

namespace ProductCatalog.CategoriesApi.Controllers;

/// <summary>
/// CRUD-микросервис для справочника категорий изделий.
/// Вся работа с БД выполнена прямо в контроллере — без дополнительных слоёв.
/// </summary>
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private static readonly string DatabasePath = ResolveDatabasePath();
    private static readonly string ConnectionString = $"Data Source={DatabasePath}";
    private static readonly object InitializationLock = new();
    private static bool _initialized;

    /// <summary>
    /// Получить список всех категорий.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        EnsureDatabase();

        var categories = new List<Category>();

        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name FROM Categories ORDER BY Name;";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categories.Add(new Category
            {
                Id = reader.GetInt64(0),
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

        EnsureDatabase();

        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name FROM Categories WHERE Name = $name;";
        command.Parameters.AddWithValue("$name", name);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound($"Категория «{name}» не найдена.");
        }

        return Ok(new Category
        {
            Id = reader.GetInt64(0),
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

        EnsureDatabase();

        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        if (await CategoryExistsAsync(connection, name))
        {
            return Conflict($"Категория «{name}» уже существует.");
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Categories (Name) VALUES ($name);";
        command.Parameters.AddWithValue("$name", name);
        await command.ExecuteNonQueryAsync();

        await using var idCommand = connection.CreateCommand();
        idCommand.CommandText = "SELECT last_insert_rowid();";
        long id = Convert.ToInt64(await idCommand.ExecuteScalarAsync());

        var category = new Category { Id = id, Name = name };

        return CreatedAtAction(
            nameof(GetByName),
            new { name },
            category);
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

        EnsureDatabase();

        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

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
        command.CommandText = "UPDATE Categories SET Name = $newName WHERE Name = $oldName;";
        command.Parameters.AddWithValue("$newName", newName);
        command.Parameters.AddWithValue("$oldName", oldName);
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

        EnsureDatabase();

        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Categories WHERE Name = $name;";
        command.Parameters.AddWithValue("$name", name);

        if (await command.ExecuteNonQueryAsync() == 0)
        {
            return NotFound($"Категория «{name}» не найдена.");
        }

        return NoContent();
    }

    private static string ResolveDatabasePath()
    {
        string? configuredPath = Environment.GetEnvironmentVariable("CATEGORIES_DB_PATH");

        return string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(AppContext.BaseDirectory, "categories.db")
            : configuredPath;
    }

    private static void EnsureDatabase()
    {
        if (_initialized)
        {
            return;
        }

        lock (InitializationLock)
        {
            if (_initialized)
            {
                return;
            }

            string? directory = Path.GetDirectoryName(DatabasePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    """
                    CREATE TABLE IF NOT EXISTS Categories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE
                    );
                    """;
                command.ExecuteNonQuery();
            }

            using (var countCommand = connection.CreateCommand())
            {
                countCommand.CommandText = "SELECT COUNT(*) FROM Categories;";
                long count = Convert.ToInt64(countCommand.ExecuteScalar());

                if (count == 0)
                {
                    SeedCategories(connection);
                }
            }

            _initialized = true;
        }
    }

    private static void SeedCategories(SqliteConnection connection)
    {
        string[] categories =
        [
            "Молочные продукты",
            "Хлебобулочные изделия",
            "Напитки",
            "Бакалея"
        ];

        using var transaction = connection.BeginTransaction();

        foreach (string category in categories)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT OR IGNORE INTO Categories (Name) VALUES ($name);";
            command.Parameters.AddWithValue("$name", category);
            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    private static async Task<bool> CategoryExistsAsync(
        SqliteConnection connection,
        string name)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Categories WHERE Name = $name;";
        command.Parameters.AddWithValue("$name", name);

        return Convert.ToInt64(await command.ExecuteScalarAsync()) > 0;
    }

    private static string Normalize(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }
}
