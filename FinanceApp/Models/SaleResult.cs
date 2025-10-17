
namespace FinanceApp.Models
{
    public sealed class SaleResult
    {
        public Product? ProductSale { get; set; }
        public string? Account {  get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public decimal Income { get; set; }
        public List<ExpensesItem>? Expenses { get; set; }
    }
}
