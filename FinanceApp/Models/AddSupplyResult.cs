using System.Collections.ObjectModel;

namespace FinanceApp.Models
{
    public sealed class AddSupplyResult
    {
        public ObservableCollection<Product>? productsResult;
        public Supply? supplyResult;
    }
}
