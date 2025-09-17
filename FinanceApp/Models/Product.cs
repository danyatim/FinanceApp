using SQLite;

namespace FinanceApp.Models;

[Table("products")]
public class Product
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal SellPrice { get; set; }
    public decimal FeePercent { get; set; } // 0..100
    public decimal BuyPrice { get; set; }
    public decimal DeliveryPrice { get; set; }

    [Ignore]
    public decimal MinSellPrice =>
        (1m - (FeePercent / 100m)) <= 0m
            ? decimal.MaxValue
            : Math.Round((BuyPrice + DeliveryPrice) / (1m - (FeePercent / 100m)), 2);

    [Ignore]
    public decimal RecommendedPrice => Math.Round(MinSellPrice * 1.30m, 2);
}