using CommunityToolkit.Maui.Views;
using FinanceApp.Models;

namespace FinanceApp.Popups;

public partial class AddProductPopup : Popup
{
    public AddProductPopup() => InitializeComponent();

    private void OnCancel(object? s, EventArgs e) => Close();

    private void OnSave(object? s, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text)) return;
        _ = int.TryParse(QtyEntry.Text, out var qty);
        _ = decimal.TryParse(SellPriceEntry.Text, out var sell);
        _ = decimal.TryParse(BuyPriceEntry.Text, out var buy);
        _ = decimal.TryParse(DeliveryPriceEntry.Text, out var del);
        _ = decimal.TryParse(FeeEntry.Text, out var fee);

        var p = new Product
        {
            Name = NameEntry.Text.Trim(),
            Quantity = qty,
            SellPrice = sell,
            BuyPrice = buy,
            DeliveryPrice = del,
            FeePercent = fee
        };
        Close(p);
    }
}