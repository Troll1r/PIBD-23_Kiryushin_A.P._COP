using Microsoft.Data.Sqlite;

namespace ProductCatalog.Data;

/// <summary>
/// Создание и инициализация локальной базы данных SQLite.
/// </summary>
public static class Database
{
    private static readonly object SyncRoot = new();
    private static bool _initialized;

    /// <summary>
    /// Путь к файлу базы данных рядом с исполняемым файлом приложения.
    /// </summary>
    public static string DatabasePath { get; } =
        Path.Combine(AppContext.BaseDirectory, "productcatalog.db");

    public static string ConnectionString => $"Data Source={DatabasePath}";

    /// <summary>
    /// Создаёт таблицы и начальные категории при первом запуске.
    /// </summary>
    public static void EnsureCreated()
    {
        if (_initialized)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            bool isNewDatabase = !File.Exists(DatabasePath);

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

                    CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT NOT NULL DEFAULT '',
                        Category TEXT NOT NULL,
                        Quantity INTEGER NULL
                    );
                    """;
                command.ExecuteNonQuery();
            }

            if (isNewDatabase)
            {
                SeedCategories(connection);
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
}
