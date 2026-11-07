namespace ProductCatalog.Data;

/// <summary>
/// Продукт магазина.
/// </summary>
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Категория изделия. Хранится как текст, без внешнего ключа.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Количество на складе. null означает, что количество не задано.
    /// </summary>
    public int? Quantity { get; set; }
}
