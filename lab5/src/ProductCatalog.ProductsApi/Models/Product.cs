namespace ProductCatalog.ProductsApi.Models;

/// <summary>
/// Продукт магазина.
/// </summary>
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Категория изделия из первого микросервиса (хранится как текст).
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Количество на складе. null — количество не задано.
    /// </summary>
    public int? Quantity { get; set; }
}

/// <summary>
/// Данные для создания или изменения продукта.
/// </summary>
public class ProductRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public int? Quantity { get; set; }
}
