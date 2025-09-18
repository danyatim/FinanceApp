using FinanceApp.Data;
using FinanceApp.Models;
using SQLite;

namespace FinanceApp.Services;

public class ReferenceService : IReferenceService
{
    private readonly IDatabase _database;

    public ReferenceService(IDatabase database)
    {
        _database = database;
    }

    public async Task<IList<Account>> GetAccountsAsync()
    {
        await _database.EnsureCreatedAsync();
        var conn = _database.GetConnection();
        return await conn.Table<Account>().OrderBy(x => x.Name).ToListAsync();
    }

    public async Task AddAccountAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        await _database.EnsureCreatedAsync();
        var conn = _database.GetConnection();
        name = name.Trim();
        var exists = await conn.Table<Account>().FirstOrDefaultAsync(a => a.Name == name);
        if (exists == null)
            await conn.InsertAsync(new Account { Name = name });
    }

    public async Task DeleteAccountAsync(int id)
    {
        await _database.EnsureCreatedAsync();
        var conn = _database.GetConnection();
        await conn.DeleteAsync<Account>(id);
    }

    public async Task<IList<Source>> GetSourcesAsync(TransactionDirection? type = null)
    {
        await _database.EnsureCreatedAsync();
        var conn = _database.GetConnection();
        var q = conn.Table<Source>();
        if (type.HasValue) q = q.Where(s => s.Type == type.Value);
        return await q.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task AddSourceAsync(string name, TransactionDirection type)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        await _database.EnsureCreatedAsync();
        var conn = _database.GetConnection();
        name = name.Trim();
        var exists = await conn.Table<Source>().FirstOrDefaultAsync(s => s.Name == name && s.Type == type);
        if (exists == null)
            await conn.InsertAsync(new Source { Name = name, Type = type });
    }

    public async Task DeleteSourceAsync(int id)
    {
        await _database.EnsureCreatedAsync();
        var conn = _database.GetConnection();
        await conn.DeleteAsync<Source>(id);
    }
}
