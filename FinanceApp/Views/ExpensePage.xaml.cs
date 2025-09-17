namespace FinanceApp.Views;

public partial class ExpensePage : ContentPage
{
    private readonly ViewModels.ExpenseViewModel _vm;
    public ExpensePage(ViewModels.ExpenseViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Appearing += async (_, __) => await _vm.LoadAsync();
    }
}