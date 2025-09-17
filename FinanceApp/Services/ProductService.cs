using FinanceApp.Data.Repositories;
using FinanceApp.Models;

namespace FinanceApp.Services;

public class ProductService : IProductService
{
    private readonly ProductRepository _repo;
    public ProductService(ProductRepository repo) => _repo = repo;

    public async Task<List<Product>> GetAllAsync(string? orderBy = null, bool ascending = true)
    {
        var list = await _repo.GetAllAsync();
        return orderBy?.ToLower() switch
        {
            "name" => ascending ? list.OrderBy(p => p.Name).ToList() : list.OrderByDescending(p => p.Name).ToList(),
            "quantity" => ascending ? list.OrderBy(p => p.Quantity).ToList() : list.OrderByDescending(p => p.Quantity).ToList(),
            "sellprice" => ascending ? list.OrderBy(p => p.SellPrice).ToList() : list.OrderByDescending(p => p.SellPrice).ToList(),
            "minsellprice" => ascending ? list.OrderBy(p => p.MinSellPrice).ToList() : list.OrderByDescending(p => p.MinSellPrice).ToList(),
            "recommendedprice" => ascending ? list.OrderBy(p => p.RecommendedPrice).ToList() : list.OrderByDescending(p => p.RecommendedPrice).ToList(),
            _ => list
        };
    }

    public Task AddAsync(Product p) => _repo.InsertAsync(p);
    public Task UpdateAsync(Product p) => _repo.UpdateAsync(p);
    public Task DeleteAsync(Product p) => _repo.DeleteAsync(p);
}