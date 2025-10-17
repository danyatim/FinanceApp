using FinanceApp.Models;

namespace FinanceApp.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync(string? orderBy = null, bool ascending = true);
    Task AddProductAsync(Product p);
    Task AddSupplyAsync(Supply p);
    Task<Supply> GetSupplyAsync(int id);
    Task UpdateAsync(Product p);
    Task DeleteAsync(Product p);

}