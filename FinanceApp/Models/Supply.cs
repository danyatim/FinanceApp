using SQLite;

namespace FinanceApp.Models
{
    [Table("Supplys")]
    public class Supply
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public int CountProduct { get; set; } = 0;
        public decimal DeliveryPrice { get; set; }

    }
}
