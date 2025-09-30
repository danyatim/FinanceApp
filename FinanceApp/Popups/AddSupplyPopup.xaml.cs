using CommunityToolkit.Maui.Views;
using FinanceApp.Models;
using FinanceApp.ViewModels;

namespace FinanceApp.Popups;

public partial class AddSupplyPopup : Popup
{
    public AddSupplyPopup(AddSupplyPopupViewModel addSupplyPopupViewModel)
    {
        InitializeComponent();
        BindingContext = addSupplyPopupViewModel;

        addSupplyPopupViewModel.RequestClose += OnVmRequestClose;

    }
    private void OnVmRequestClose(object? sender, AddSupplyResult? result)
    {
        if (BindingContext is AddSupplyPopupViewModel vm)
            vm.RequestClose -= OnVmRequestClose;

        // На всякий случай на UI-потоке
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Close(result);
        });
    }
}