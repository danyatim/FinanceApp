using CommunityToolkit.Maui.Views;
using FinanceApp.Models;
using FinanceApp.ViewModels;

namespace FinanceApp.Popups;

public partial class AddSalePopup : Popup
{
	public AddSalePopup(AddSalePopupViewModel addSalePopup)
	{
		InitializeComponent();
		BindingContext = addSalePopup;

        addSalePopup.RequestClose += OnVmRequestClose;
        this.Opened += async (_, __) =>
        {
            await addSalePopup.Load();
        };

    }
    private void OnVmRequestClose(object? sender, SaleResult? result)
    {
        if (BindingContext is AddSalePopupViewModel vm)
            vm.RequestClose -= OnVmRequestClose;

        // На всякий случай на UI-потоке
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Close(result);
        });
    }
}