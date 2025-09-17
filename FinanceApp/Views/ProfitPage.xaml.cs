namespace FinanceApp.Views;

public partial class ProfitPage : ContentPage
{
    private readonly ViewModels.ProfitViewModel _vm;
    public ProfitPage(ViewModels.ProfitViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Appearing += async (_, __) => await _vm.LoadAsync();
    }
}