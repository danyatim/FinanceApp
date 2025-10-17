using System.Collections.ObjectModel;

namespace FinanceApp.Models
{
    public class AddSupplyResult
    {
        public ObservableCollection<Product>? ProductsResult { get; set; }
        public Supply? SupplyResult { get; set; }
    }
}
