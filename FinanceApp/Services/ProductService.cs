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
            "color" => ascending ? list.OrderBy(p => p.Color).ToList() : list.OrderByDescending(p => p.Color).ToList(),
            "size" => ascending ? list.OrderBy(p => p.Size).ToList() : list.OrderByDescending(p => p.Size).ToList(),
            "quantity" => ascending ? list.OrderBy(p => p.Quantity).ToList() : list.OrderByDescending(p => p.Quantity).ToList(),
            "sellprice" => ascending ? list.OrderBy(p => p.SellPrice).ToList() : list.OrderByDescending(p => p.SellPrice).ToList(),
            "minsellprice" => ascending ? list.OrderBy(p => p.MinSellPrice).ToList() : list.OrderByDescending(p => p.MinSellPrice).ToList(),
            "recommendedprice" => ascending ? list.OrderBy(p => p.RecommendedPrice).ToList() : list.OrderByDescending(p => p.RecommendedPrice).ToList(),
            _ => list
        };
    }

    public Task AddProductAsync(Product p) => _repo.InsertProductAsync(p);
    public Task AddSupplyAsync(Supply p) => _repo.InsertSupplyAsync(p);
    public Task<Supply> GetSupplyAsync(int id) => _repo.GetSupplyAsync(id);
    public Task UpdateAsync(Product p) => _repo.UpdateAsync(p);
    public Task DeleteAsync(Product p) => _repo.DeleteAsync(p);
}