using CommunityToolkit.Maui;
using FinanceApp.ViewModels;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FinanceApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .UseLiveCharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Montserrat-Bold.ttf", "Montserrat-Bold");
                fonts.AddFont("Montserrat-Light.ttf", "MontserratLight");
                fonts.AddFont("Montserrat-SemiBold.ttf", "MontserratSemiBold");
                fonts.AddFont("Montserrat-Thin.ttf", "MontserratThin");

                fonts.AddFont("arialmt.ttf", "Arial");

                fonts.AddFont("georgia.ttf", "Georgia");
            });
        builder.ConfigureMauiHandlers(handlers =>
        {
#if WINDOWS
            EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                if (handler.PlatformView is TextBox tb)
                {
                    // 1) Шрифт плейсхолдера = шрифт TextBox
                    tb.FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Arial");

                    // 2) Цвета плейсхолдера по состояниям
                    tb.Resources["TextControlPlaceholderForeground"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(160, 200, 200, 200));
                    tb.Resources["TextControlPlaceholderForegroundPointerOver"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(180, 210, 210, 210));
                    tb.Resources["TextControlPlaceholderForegroundFocused"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 230, 230, 230));
                    tb.Resources["TextControlPlaceholderForegroundDisabled"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(120, 180, 180, 180));

                    // 3) Фон по состояниям
                    tb.Resources["TextControlBackground"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(30, 255, 255, 255));  // Normal
                    tb.Resources["TextControlBackgroundPointerOver"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(40, 255, 255, 255));  // Hover
                    tb.Resources["TextControlBackgroundFocused"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(50, 255, 255, 255));  // Focused
                    tb.Resources["TextControlBackgroundDisabled"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(20, 255, 255, 255));  // Disabled

                    // 4) Рамка по состояниям
                    var border = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 1, 115, 237));
                    tb.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
                    tb.Resources["TextControlBorderBrush"] = border;
                    tb.Resources["TextControlBorderBrushPointerOver"] = border;
                    tb.Resources["TextControlBorderBrushFocused"] = border;
                    tb.Resources["TextControlBorderBrushDisabled"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(60, 1, 115, 237));

                    // 5) Цвет текста по состояниям (при необходимости)
                    tb.Resources["TextControlForeground"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                    tb.Resources["TextControlForegroundPointerOver"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                    tb.Resources["TextControlForegroundFocused"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                    tb.Resources["TextControlForegroundDisabled"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(160, 0, 0, 0));

                    // 6) Скругление, выделение
                    tb.Resources["ControlCornerRadius"] = new Microsoft.UI.Xaml.CornerRadius(0);
                    tb.Resources["TextControlSelectionForeground"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                    tb.Resources["TextControlSelectionHighlightColor"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 184, 184, 184));
                }
            });
#endif
        });

        LiveCharts.Configure(cfg => cfg.AddSkiaSharp());

        // Profile
        builder.Services.AddSingleton<Services.IProfileService, Services.ProfileService>();

        // БД теперь зависит от профиля
        builder.Services.AddSingleton<Data.IDatabase>(sp =>
            new Data.Database(sp.GetRequiredService<Services.IProfileService>()));

        // Repositories
        builder.Services.AddSingleton<Data.Repositories.TransactionRepository>();
        builder.Services.AddSingleton<Data.Repositories.ProductRepository>();

        // Services
        builder.Services.AddSingleton<Services.IDateRangeService, Services.DateRangeService>();
        builder.Services.AddSingleton<Services.ITransactionService, Services.TransactionService>();
        builder.Services.AddSingleton<Services.IProductService, Services.ProductService>();
        builder.Services.AddSingleton<Services.IReferenceService, Services.ReferenceService>();

        // VM
        builder.Services.AddSingleton<ViewModels.MainViewModel>();
        builder.Services.AddTransient<ViewModels.RevenueViewModel>();
        builder.Services.AddTransient<ViewModels.ExpenseViewModel>();
        builder.Services.AddTransient<ViewModels.ProfitViewModel>();
        builder.Services.AddSingleton<ViewModels.WarehouseViewModel>();
        builder.Services.AddTransient<ViewModels.SettingsViewModel>();

        //Popup
        builder.Services.AddTransient<ViewModels.AddSupplyPopupViewModel>();
        builder.Services.AddTransientPopup<Popups.AddSupplyPopup, AddSupplyPopupViewModel>();

        // Pages
        builder.Services.AddSingleton<Views.MainPage>();
        builder.Services.AddTransient<Views.RevenuePage>();
        builder.Services.AddTransient<Views.ExpensePage>();
        builder.Services.AddTransient<Views.ProfitPage>();
        builder.Services.AddSingleton<Views.WarehousePage>();
        builder.Services.AddSingleton<Views.ProfilePage>();

        return builder.Build();
    }
}
