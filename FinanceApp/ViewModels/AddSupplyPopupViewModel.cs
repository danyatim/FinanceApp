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
            RequestClose?.Invoke(this, new AddSupplyResult { productsResult = null, supplyResult = null });
        }

        [RelayCommand]
        private void AddProduct()
        {
            if (string.IsNullOrWhiteSpace(ProductName) &&
                string.IsNullOrWhiteSpace(ProductColor) &&
                string.IsNullOrWhiteSpace(ProductSize)) return;

            _ = int.TryParse(ProductQuantity.ToString(), out var quantity);
            _ = decimal.TryParse(ProductSellPrice.ToString(), out var sellPrice);
            _ = decimal.TryParse(ProductBuyPrice.ToString(), out var buyPrice);
            _ = decimal.TryParse(SupplyDeliveryPrice.ToString(), out var deliveryPrice);
            _ = decimal.TryParse(ProductFeePercent.ToString(), out var feePercent);

            var p = new Product
            {
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
        }

        [RelayCommand]
        private void OnSave()
        {
            _ = int.TryParse(CountProduct.ToString(), out var countProduct);
            _ = decimal.TryParse(SupplyDeliveryPrice.ToString(), out var deliveryPrice);

            var supply = new Supply
            {
                CountProduct = countProduct,
                DeliveryPrice = deliveryPrice,
            };
            foreach (var product in Products)
            {
                product.DeliveryPrice = Math.Round(deliveryPrice / countProduct, 2);
                product.Supply = supply;
            }
            RequestClose?.Invoke(this, new AddSupplyResult { productsResult = Products, supplyResult = supply });
        }
    }
}
