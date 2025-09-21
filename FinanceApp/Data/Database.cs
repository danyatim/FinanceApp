using SQLite;
using FinanceApp.Models;
using FinanceApp.Services;
using System.Threading;

namespace FinanceApp.Data;

public interface IDatabase
{
    SQLiteAsyncConnection GetConnection();

    Task EnsureCreatedAsync();

    Task SetProfileAsync(string name);
    string CurrentProfileName { get; }
}

public class Database : IDatabase
{
    private readonly IProfileService _profiles;

    private SQLiteAsyncConnection _conn = default!;
    private Task? _initTask;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public string CurrentProfileName { get; private set; } = "default";

    public Database(IProfileService profiles)
    {
        _profiles = profiles;

        var name = _profiles.GetCurrentProfileName() ?? "default";
        CurrentProfileName = name;
        var path = _profiles.GetDbFilePath(name);

        _conn = CreateConnection(path);
        // запуск инициализации лениво — через EnsureCreatedAsync
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

    public async Task SetProfileAsync(string name)
    {
        name = string.IsNullOrWhiteSpace(name) ? "default" : name.Trim();
        if (string.Equals(CurrentProfileName, name, StringComparison.OrdinalIgnoreCase))
        {
            await EnsureCreatedAsync();
            return;
        }

        CurrentProfileName = name;

        try { await _conn.CloseAsync(); } catch { /* ignore */ }

        var path = _profiles.GetDbFilePath(name);
        _conn = CreateConnection(path);

        // сбрасываем задачу инициализации и запускаем заново
        _initTask = null;
        await EnsureCreatedAsync();
    }

    private SQLiteAsyncConnection CreateConnection(string path)
    {
        return new SQLiteAsyncConnection(
            path,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache,
            storeDateTimeAsTicks: false);
    }

    private async Task InitAsync()
    {
        await _conn.CreateTableAsync<Transaction>();
        await _conn.CreateTableAsync<Product>();
        await _conn.CreateTableAsync<Account>();
        await _conn.CreateTableAsync<Source>();

        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_date ON transactions(Date)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_direction ON transactions(Direction)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_account ON transactions(Account)");
        await _conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transactions_source ON transactions(Source)");
    }
}

