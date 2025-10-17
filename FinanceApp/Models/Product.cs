using SQLite;
using System.Diagnostics;

namespace FinanceApp.Models;

[Table("Products")]
public class Product
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
    public string Size { get; set; } = "";
    public int Quantity { get; set; }
    public decimal SellPrice { get; set; }
    public decimal FeePercent { get; set; } // 0..100
    public decimal BuyPrice { get; set; }
    public decimal DeliveryPrice { get; set; }
    public decimal OzonExpensesSum { get; set; } = decimal.Zero;
    public decimal OzonPercent { get; set; } = decimal.Zero;
    public int SupplyId { get; set; }

    [Ignore]
    public Supply? Supply { get; set; }

    [Ignore]
    public decimal MinSellPrice =>
        (1m - (FeePercent / 100m)) <= 0m
            ? decimal.MaxValue
            : Math.Round((BuyPrice + (DeliveryPrice / Quantity)) + OzonExpensesSum, 2);

    [Ignore]
    public decimal RecommendedPrice => Math.Round(MinSellPrice * 1.30m, 2);
}