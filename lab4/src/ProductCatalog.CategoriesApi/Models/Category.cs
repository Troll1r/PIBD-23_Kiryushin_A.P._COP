namespace ProductCatalog.CategoriesApi.Models;

/// <summary>
/// Категория изделий — простая справочная сущность с одним текстовым полем.
/// </summary>
public class Category
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Данные для создания или изменения категории.
/// </summary>
public class CategoryRequest
{
    public string Name { get; set; } = string.Empty;
}
