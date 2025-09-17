using FinanceApp.Models;

namespace FinanceApp.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync(string? orderBy = null, bool ascending = true);
    Task AddAsync(Product p);
    Task UpdateAsync(Product p);
    Task DeleteAsync(Product p);
}