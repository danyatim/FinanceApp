using SQLite;

namespace FinanceApp.Models;

[Table("Accounts")]
public class Account
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Indexed(Unique = true)] public string Name { get; set; } = "";
}
