using SQLite;

namespace FinanceApp.Models;

[Table("transactions")]
public class Transaction
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionDirection Direction { get; set; }
    public string Account { get; set; } = "";
    public string Source { get; set; } = "";
    public string? Note { get; set; }
}