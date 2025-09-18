using SQLite;
using System.Diagnostics;

namespace FinanceApp.Data;

public interface IDatabase
{
    SQLiteAsyncConnection GetConnection();
    Task EnsureCreatedAsync();
}

public class Database : IDatabase
{
    private readonly SQLiteAsyncConnection _conn;
    private Task? _initTask;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public Database()
    {
        var dir = FileSystem.AppDataDirectory;
        var dbPath = Path.Combine(dir, "finance.sqlite3");
        Debug.WriteLine(dbPath);
        _conn = new SQLiteAsyncConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache,
            storeDateTimeAsTicks: false // ВАЖНО: сохранять DateTime как ISO-8601 TEXT
        );
        _initTask = InitAsync();
    }

    private async Task InitAsync()
    {
        await _conn.CreateTableAsync<Models.Transaction>();
        await _conn.CreateTableAsync<Models.Product>();
        await _conn.CreateTableAsync<Models.Account>();
        await _conn.CreateTableAsync<Models.Source>();

        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_date ON transactions(Date)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_direction ON transactions(Direction)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_account ON transactions(Account)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_source ON transactions(Source)");
    }

    public SQLiteAsyncConnection GetConnection() => _conn;

    public async Task EnsureCreatedAsync()
    {
        if (_initTask == null)
        {
            await _initLock.WaitAsync();
            try
            {
                _initTask ??= InitAsync();
            }
            finally { _initLock.Release(); }
        }
        await _initTask!;
    }
}