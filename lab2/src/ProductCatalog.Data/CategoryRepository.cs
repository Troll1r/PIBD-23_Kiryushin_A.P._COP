using Microsoft.Data.Sqlite;

namespace ProductCatalog.Data;

/// <summary>
/// Работа со справочником категорий продуктов.
/// </summary>
public class CategoryRepository
{
    public CategoryRepository()
    {
        Database.EnsureCreated();
    }

    public List<string> GetAll()
    {
        var result = new List<string>();

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Name FROM Categories ORDER BY Name COLLATE NOCASE;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }

        return result;
    }

    public bool Exists(string name)
    {
        name = Normalize(name);

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Categories WHERE Name = $name;";
        command.Parameters.AddWithValue("$name", name);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public void Add(string name)
    {
        name = Normalize(name);

        if (Exists(name))
        {
            throw new InvalidOperationException($"Категория «{name}» уже существует.");
        }

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Categories (Name) VALUES ($name);";
        command.Parameters.AddWithValue("$name", name);
        command.ExecuteNonQuery();
    }

    public void Update(string oldName, string newName)
    {
        oldName = Normalize(oldName);
        newName = Normalize(newName);

        if (string.Equals(oldName, newName, StringComparison.Ordinal))
        {
            return;
        }

        if (Exists(newName))
        {
            throw new InvalidOperationException($"Категория «{newName}» уже существует.");
        }

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "UPDATE Categories SET Name = $newName WHERE Name = $oldName;";
            command.Parameters.AddWithValue("$newName", newName);
            command.Parameters.AddWithValue("$oldName", oldName);
            command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "UPDATE Products SET Category = $newName WHERE Category = $oldName;";
            command.Parameters.AddWithValue("$newName", newName);
            command.Parameters.AddWithValue("$oldName", oldName);
            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public void Delete(string name)
    {
        name = Normalize(name);

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Categories WHERE Name = $name;";
        command.Parameters.AddWithValue("$name", name);
        command.ExecuteNonQuery();
    }

    private static string Normalize(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Название категории не может быть пустым.", nameof(name));
        }

        return name.Trim();
    }
}
