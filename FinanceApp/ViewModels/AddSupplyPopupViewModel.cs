using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using System.Collections.ObjectModel;

namespace FinanceApp.ViewModels
{
    public partial class AddSupplyPopupViewModel(IPopupService popupService) : ObservableObject
    {
        [ObservableProperty] ObservableCollection<Product> products = [];

        public event EventHandler<AddSupplyResult?>? RequestClose;

        [ObservableProperty] private int countProduct = 0;
        [ObservableProperty] private decimal supplyDeliveryPrice;

        [ObservableProperty] private string? productArticle;
        [ObservableProperty] private string? productName;
        [ObservableProperty] private string? productColor;
        [ObservableProperty] private string? productSize;
        [ObservableProperty] private int productQuantity;
        [ObservableProperty] private decimal productSellPrice;
        [ObservableProperty] private decimal productBuyPrice;
        [ObservableProperty] private decimal productFeePercent;

        readonly IPopupService popupService = popupService;

        [RelayCommand]
        private void OnCancel()
        {
            Products.Clear();
            RequestClose?.Invoke(this, new AddSupplyResult { ProductsResult = null, SupplyResult = null });
        }

        [RelayCommand]
        private void AddProduct()
        {
            if (string.IsNullOrWhiteSpace(ProductName) ||
                string.IsNullOrWhiteSpace(ProductColor) ||
                string.IsNullOrWhiteSpace(ProductSize)) return;

            _ = int.TryParse(ProductQuantity.ToString(), out var quantity);
            _ = decimal.TryParse(ProductSellPrice.ToString(), out var sellPrice);
            _ = decimal.TryParse(ProductBuyPrice.ToString(), out var buyPrice);
            _ = decimal.TryParse(SupplyDeliveryPrice.ToString(), out var deliveryPrice);
            _ = decimal.TryParse(ProductFeePercent.ToString(), out var feePercent);

            var p = new Product
            {
                Article = string.IsNullOrWhiteSpace(ProductArticle) ? "" : ProductArticle.Trim(),
                Name = ProductName.Trim(),
                Color = ProductColor.Trim(),
                Size = ProductSize.Trim(),
                Quantity = quantity,
                SellPrice = sellPrice,
                BuyPrice = buyPrice,
                DeliveryPrice = deliveryPrice,
                FeePercent = feePercent,
            };
            Products.Add(p);
            ProductArticle = "";
            ProductColor = "";
            ProductQuantity = 0;
        }

        [RelayCommand]
        private void OnSave()
        {
            var count = CountProduct > 0 ? CountProduct : Products.Sum(p => Math.Max(1, p.Quantity));
            var deliveryPrice = SupplyDeliveryPrice;

            if (count <= 0)
            {
                RequestClose?.Invoke(this, new AddSupplyResult { ProductsResult = Products, SupplyResult = null });
                return;
            }

            var supply = new Supply
            {
                CountProduct = count,
                DeliveryPrice = deliveryPrice,
            };

            foreach (var product in Products)
            {
                var pieces = Math.Max(1, product.Quantity);
                product.DeliveryPrice = Math.Round((deliveryPrice / count) * pieces, 2);
                product.SupplyId = supply.Id;
                product.Supply = supply;
            }

            RequestClose?.Invoke(this, new AddSupplyResult { ProductsResult = Products, SupplyResult = supply });
        }
    }
}
