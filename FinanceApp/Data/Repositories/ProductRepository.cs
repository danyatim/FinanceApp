using FinanceApp.Models;
using SQLite;

namespace FinanceApp.Data.Repositories;

public class ProductRepository
{
    private readonly IDatabase _database;
    private SQLiteAsyncConnection Conn => _database.GetConnection();

    public ProductRepository(IDatabase database) => _database = database;

    public async Task<int> InsertAsync(Product p)
    {
        await _database.EnsureCreatedAsync();
        return await Conn.InsertAsync(p);
    }

    public async Task<int> UpdateAsync(Product p)
    {
        await _database.EnsureCreatedAsync();
        return await Conn.UpdateAsync(p);
    }

    public async Task<int> DeleteAsync(Product p)
    {
        await _database.EnsureCreatedAsync();
        return await Conn.DeleteAsync(p);
    }

    public async Task<List<Product>> GetAllAsync()
    {
        await _database.EnsureCreatedAsync();
        return await Conn.Table<Product>().OrderBy(p => p.Name).ToListAsync();
    }
}
