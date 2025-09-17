using SQLite;

namespace FinanceApp.Data;

public interface IDatabase
{
    SQLiteAsyncConnection GetConnection();
}

public class Database : IDatabase
{
    private readonly SQLiteAsyncConnection _conn;

    public Database()
    {
        var dir = FileSystem.AppDataDirectory;
        var dbPath = Path.Combine(dir, "finance.sqlite3");
        _conn = new SQLiteAsyncConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache,
            storeDateTimeAsTicks: false // ВАЖНО: сохранять DateTime как ISO-8601 TEXT
        );
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await _conn.CreateTableAsync<Models.Transaction>();
        await _conn.CreateTableAsync<Models.Product>();

        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_date ON transactions(Date)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_direction ON transactions(Direction)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_account ON transactions(Account)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_source ON transactions(Source)");
    }

    public SQLiteAsyncConnection GetConnection() => _conn;
}