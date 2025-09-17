namespace FinanceApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.RevenuePage), typeof(Views.RevenuePage));
        Routing.RegisterRoute(nameof(Views.ExpensePage), typeof(Views.ExpensePage));
        Routing.RegisterRoute(nameof(Views.ProfitPage), typeof(Views.ProfitPage));
    }
}