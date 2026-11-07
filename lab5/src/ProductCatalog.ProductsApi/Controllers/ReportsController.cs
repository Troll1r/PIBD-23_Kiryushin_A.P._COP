using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ProductCatalog.ProductsApi.Data;
using ProductCatalog.ProductsApi.Models;
using ProductCatalog.ProductsApi.Reports;

namespace ProductCatalog.ProductsApi.Controllers;

/// <summary>
/// Второй контроллер микросервиса — предоставление файлов отчётов.
/// Файлы формируются сразу при вызове метода, без плагинной инфраструктуры.
/// </summary>
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private const string ExcelContentType =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private const string PdfContentType = "application/pdf";

    private const string WordContentType =
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    private readonly NpgsqlDataSource _dataSource;

    public ReportsController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Excel по продуктам, которые есть в наличии.
    /// </summary>
    [HttpGet("products-in-stock")]
    [Produces(ExcelContentType)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsInStock()
    {
        await ProductsDatabase.EnsureCreatedAsync(_dataSource);

        var products = new List<Product>();

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, Name, Description, Category, Quantity
            FROM Products
            WHERE Quantity > 0
            ORDER BY Name;
            """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(ReadProduct(reader));
        }

        byte[] content = ReportGenerator.CreateInStockExcel(products);

        return File(content, ExcelContentType, "products-in-stock.xlsx");
    }

    /// <summary>
    /// PDF с круговой диаграммой по продуктам, которых нет на складе.
    /// </summary>
    [HttpGet("out-of-stock-pie")]
    [Produces(PdfContentType)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutOfStockPie()
    {
        await ProductsDatabase.EnsureCreatedAsync(_dataSource);

        var groups = new List<(string Category, int Count)>();

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Category, COUNT(*)
            FROM Products
            WHERE Quantity IS NULL OR Quantity <= 0
            GROUP BY Category
            ORDER BY Category;
            """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            groups.Add((
                reader.GetString(0),
                checked((int)reader.GetInt64(1))));
        }

        byte[] content = ReportGenerator.CreateOutOfStockPiePdf(groups);

        return File(content, PdfContentType, "out-of-stock-pie.pdf");
    }

    /// <summary>
    /// Word со всеми продуктами.
    /// </summary>
    [HttpGet("all-products")]
    [Produces(WordContentType)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProducts()
    {
        await ProductsDatabase.EnsureCreatedAsync(_dataSource);

        var products = new List<Product>();

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, Name, Description, Category, Quantity
            FROM Products
            ORDER BY Id;
            """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(ReadProduct(reader));
        }

        byte[] content = ReportGenerator.CreateAllProductsWord(products);

        return File(content, WordContentType, "all-products.docx");
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
