using Microsoft.Data.Sqlite;

namespace ProductCatalog.Data;

/// <summary>
/// Работа со списком продуктов.
/// </summary>
public class ProductRepository
{
    public ProductRepository()
    {
        Database.EnsureCreated();
    }

    public List<Product> GetAll()
    {
        return Query(
            """
            SELECT Id, Name, Description, Category, Quantity
            FROM Products
            ORDER BY Category COLLATE NOCASE,
                     CASE WHEN Quantity IS NULL THEN -1 ELSE Quantity END,
                     Name COLLATE NOCASE,
                     Id;
            """);
    }

    public List<Product> GetByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return [];
        }

        return Query(
            """
            SELECT Id, Name, Description, Category, Quantity
            FROM Products
            WHERE Category = $category
            ORDER BY CASE WHEN Quantity IS NULL THEN -1 ELSE Quantity END,
                     Name COLLATE NOCASE,
                     Id;
            """,
            ("$category", category.Trim()));
    }

    public List<string> GetDistinctCategories()
    {
        var result = new List<string>();

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT DISTINCT Category
            FROM Products
            WHERE TRIM(Category) <> ''
            ORDER BY Category COLLATE NOCASE;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }

        return result;
    }

    public int Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        Validate(product);

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO Products (Name, Description, Category, Quantity)
                VALUES ($name, $description, $category, $quantity);
                """;
            command.Parameters.AddWithValue("$name", product.Name.Trim());
            command.Parameters.AddWithValue("$description", product.Description.Trim());
            command.Parameters.AddWithValue("$category", product.Category.Trim());
            command.Parameters.AddWithValue("$quantity", (object?)product.Quantity ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        using var idCommand = connection.CreateCommand();
        idCommand.CommandText = "SELECT last_insert_rowid();";
        return Convert.ToInt32(idCommand.ExecuteScalar());
    }

    public void Update(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        Validate(product);

        if (product.Id <= 0)
        {
            throw new ArgumentException("Идентификатор продукта не задан.", nameof(product));
        }

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE Products
            SET Name = $name,
                Description = $description,
                Category = $category,
                Quantity = $quantity
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$name", product.Name.Trim());
        command.Parameters.AddWithValue("$description", product.Description.Trim());
        command.Parameters.AddWithValue("$category", product.Category.Trim());
        command.Parameters.AddWithValue("$quantity", (object?)product.Quantity ?? DBNull.Value);
        command.Parameters.AddWithValue("$id", product.Id);

        if (command.ExecuteNonQuery() == 0)
        {
            throw new InvalidOperationException("Продукт не найден в базе данных.");
        }
    }

    public void Delete(int id)
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Products WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static List<Product> Query(
        string sql,
        params (string Name, object Value)[] parameters)
    {
        var result = new List<Product>();

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Category = reader.GetString(3),
                Quantity = reader.IsDBNull(4) ? null : reader.GetInt32(4)
            });
        }

        return result;
    }

    private static void Validate(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new ArgumentException("Название продукта не может быть пустым.", nameof(product));
        }

        if (string.IsNullOrWhiteSpace(product.Category))
        {
            throw new ArgumentException("Категория продукта не может быть пустой.", nameof(product));
        }
    }
}
