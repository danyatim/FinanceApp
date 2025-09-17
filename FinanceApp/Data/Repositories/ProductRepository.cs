using FinanceApp.Models;
using SQLite;

namespace FinanceApp.Data.Repositories;

public class ProductRepository
{
    private readonly SQLiteAsyncConnection _db;
    public ProductRepository(IDatabase database) => _db = database.GetConnection();

    public Task<int> InsertAsync(Product p) => _db.InsertAsync(p);
    public Task<int> UpdateAsync(Product p) => _db.UpdateAsync(p);
    public Task<int> DeleteAsync(Product p) => _db.DeleteAsync(p);

    public Task<List<Product>> GetAllAsync() =>
        _db.Table<Product>().OrderBy(p => p.Name).ToListAsync();
}