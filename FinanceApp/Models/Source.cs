using SQLite;

namespace FinanceApp.Models;

[Table("Sources")]
public class Source
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed] public TransactionDirection Type { get; set; }
    [Indexed] public string Name { get; set; } = "";
}
