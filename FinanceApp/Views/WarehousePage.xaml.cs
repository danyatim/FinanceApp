namespace FinanceApp.Views;

public partial class WarehousePage : ContentPage
{
    private readonly ViewModels.WarehouseViewModel _vm;
    public WarehousePage(ViewModels.WarehouseViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Appearing += async (_, __) => await _vm.LoadAsync();
    }
}