namespace FinanceApp.Views;

public partial class RevenuePage : ContentPage
{
    private readonly ViewModels.RevenueViewModel _vm;
    public RevenuePage(ViewModels.RevenueViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Appearing += async (_, __) => await _vm.LoadAsync();
    }
}