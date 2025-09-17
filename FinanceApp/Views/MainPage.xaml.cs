namespace FinanceApp.Views;

public partial class MainPage : ContentPage
{
    private readonly ViewModels.MainViewModel _vm;
    public MainPage(ViewModels.MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Appearing += async (_, __) => await _vm.LoadAsync();
    }
}