using System.Globalization;

namespace FinanceApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Локаль по умолчанию — Россия
        var culture = CultureInfo.GetCultureInfo("ru-RU");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;


    }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}