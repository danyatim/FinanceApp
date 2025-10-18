namespace FinanceApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.RevenuePage), typeof(Views.RevenuePage));
        Routing.RegisterRoute(nameof(Views.ExpensePage), typeof(Views.ExpensePage));
        Routing.RegisterRoute(nameof(Views.ProfitPage), typeof(Views.ProfitPage));

        Routing.RegisterRoute(nameof(Views.ProfilePage), typeof(Views.ProfilePage));
        Routing.RegisterRoute(nameof(Views.MainPage), typeof(Views.MainPage));
        Routing.RegisterRoute(nameof(Views.WarehousePage), typeof(Views.WarehousePage));

        //// �����������: ������� �� ������� ���������, ���� ������� �� ������
        //var prof = Application.Current?.Windows.FirstOrDefault().Services.GetService<FinanceApp.Services.IProfileService>();
        //if (prof?.GetCurrentProfileName() is null)
        //{
        //    // CurrentItem � ������ ������� (�������)
        //    // ������ ������������� �� ������, Shell ��� �� ���.
        //}
    }
}