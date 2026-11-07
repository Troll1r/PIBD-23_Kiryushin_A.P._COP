using Npgsql;

namespace ProductCatalog.ProductsApi.Data;

/// <summary>
/// Создание общей схемы БД (Categories — данные первого сервиса,
/// Products — данные второго сервиса) и начальных категорий.
/// </summary>
public static class ProductsDatabase
{
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);
    private static bool _initialized;

    public static async Task EnsureCreatedAsync(NpgsqlDataSource dataSource)
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
                    await using var connection = await dataSource.OpenConnectionAsync();

                    await using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            """
                            CREATE TABLE IF NOT EXISTS Categories (
                                Id SERIAL PRIMARY KEY,
                                Name TEXT NOT NULL UNIQUE
                            );

                            CREATE TABLE IF NOT EXISTS Products (
                                Id SERIAL PRIMARY KEY,
                                Name TEXT NOT NULL,
                                Description TEXT NOT NULL DEFAULT '',
                                Category TEXT NOT NULL,
                                Quantity INTEGER NULL
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
                "Не удалось подключиться к PostgreSQL и создать таблицы.");
        }
        finally
        {
            InitializationLock.Release();
        }
    }
}
