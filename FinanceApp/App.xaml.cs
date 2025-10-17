using Microsoft.Maui;
using System.Diagnostics;
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
    protected override Window CreateWindow(IActivationState? activationState) =>
        new Window(new AppShell())
        {
            Width = 1920,
            Height = 1080,
            MinimumWidth = 1920,
            MinimumHeight = 1080,
            X = 0,
            Y = 0
        };
}