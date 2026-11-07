using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ProductCatalog.ProductsApi.Data;
using ProductCatalog.ProductsApi.Models;

namespace ProductCatalog.ProductsApi.Controllers;

/// <summary>
/// CRUD-операции по продуктам с валидацией данных.
/// Категория проверяется по общей таблице Categories первого микросервиса.
/// </summary>
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly NpgsqlDataSource _dataSource;

    public ProductsController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Получить все продукты.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        await ProductsDatabase.EnsureCreatedAsync(_dataSource);

        var products = new List<Product>();

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, Name, Description, Category, Quantity
            FROM Products
            ORDER BY Category, Name, Id;
            """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(ReadProduct(reader));
        }

        return Ok(products);
    }

    /// <summary>
    /// Получить продукт по идентификатору.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        await ProductsDatabase.EnsureCreatedAsync(_dataSource);

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, Name, Description, Category, Quantity
            FROM Products
            WHERE Id = @id;
            """;
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound($"Продукт с Id={id} не найден.");
        }

        return Ok(ReadProduct(reader));
    }

    /// <summary>
    /// Добавить новый продукт.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> Create([FromBody] ProductRequest request)
    {
        if (!TryNormalize(
                request,
                out string name,
                out string description,
                out string category,
                out string error))
        {
            return BadRequest(error);
        }

        await ProductsDatabase.EnsureCreatedAsync(_dataSource);

        await using var connection = await _dataSource.OpenConnectionAsync();

        if (!await CategoryExistsAsync(connection, category))
        {
            return BadRequest($"Категория «{category}» не найдена в справочнике.");
        }

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO Products (Name, Description, Category, Quantity)
            VALUES (@name, @description, @category, @quantity)
            RETURNING Id;
            """;
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("description", description);
        command.Parameters.AddWithValue("category", category);
        command.Parameters.AddWithValue("quantity", (object?)request?.Quantity ?? DBNull.Value);

        int id = Convert.ToInt32(await command.ExecuteScalarAsync());
        var product = new Product
        {
            Id = id,
            Name = name,
            Description = description,
            Category = category,
            Quantity = request?.Quantity
        };

        return CreatedAtAction(nameof(GetById), new { id }, product);
    }

    /// <summary>
    /// Изменить существующий продукт.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] ProductRequest request)
    {
        if (!TryNormalize(
                request,
                out string name,
                out string description,
                out string category,
                out string error))
        {
            return BadRequest(error);
        }

        await ProductsDatabase.EnsureCreatedAsync(_dataSource);

        await using var connection = await _dataSource.OpenConnectionAsync();

        if (!await ProductExistsAsync(connection, id))
        {
            return NotFound($"Продукт с Id={id} не найден.");
        }

        if (!await CategoryExistsAsync(connection, category))
        {
            return BadRequest($"Категория «{category}» не найдена в справочнике.");
        }

        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE Products
            SET Name = @name,
                Description = @description,
                Category = @category,
                Quantity = @quantity
            WHERE Id = @id;
            """;
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("description", description);
        command.Parameters.AddWithValue("category", category);
        command.Parameters.AddWithValue("quantity", (object?)request?.Quantity ?? DBNull.Value);
        command.Parameters.AddWithValue("id", id);
        await command.ExecuteNonQueryAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить продукт.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await ProductsDatabase.EnsureCreatedAsync(_dataSource);

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Products WHERE Id = @id;";
        command.Parameters.AddWithValue("id", id);

        if (await command.ExecuteNonQueryAsync() == 0)
        {
            return NotFound($"Продукт с Id={id} не найден.");
        }

        return NoContent();
    }

    private static bool TryNormalize(
        ProductRequest? request,
        out string name,
        out string description,
        out string category,
        out string error)
    {
        name = request?.Name?.Trim() ?? string.Empty;
        description = request?.Description?.Trim() ?? string.Empty;
        category = request?.Category?.Trim() ?? string.Empty;
        error = string.Empty;

        if (name.Length == 0)
        {
            error = "Название продукта не может быть пустым.";
            return false;
        }

        if (category.Length == 0)
        {
            error = "Категория продукта не может быть пустой.";
            return false;
        }

        if (request?.Quantity is < 0)
        {
            error = "Количество на складе не может быть отрицательным.";
            return false;
        }

        return true;
    }

    private static async Task<bool> ProductExistsAsync(
        NpgsqlConnection connection,
        int id)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Products WHERE Id = @id;";
        command.Parameters.AddWithValue("id", id);

        return Convert.ToInt64(await command.ExecuteScalarAsync()) > 0;
    }

    private static async Task<bool> CategoryExistsAsync(
        NpgsqlConnection connection,
        string category)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Categories WHERE Name = @name;";
        command.Parameters.AddWithValue("name", category);

        return Convert.ToInt64(await command.ExecuteScalarAsync()) > 0;
    }

    private static Product ReadProduct(NpgsqlDataReader reader)
    {
        return new Product
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            Category = reader.GetString(3),
            Quantity = reader.IsDBNull(4) ? null : reader.GetInt32(4)
        };
    }
}
