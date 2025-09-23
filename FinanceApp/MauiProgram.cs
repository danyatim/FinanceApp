using CommunityToolkit.Maui;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Maui.Handlers;
using SkiaSharp.Views.Maui.Controls.Hosting;


#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI; // для Colors.Transparent; в WinUI 3 можно также Microsoft.UI.Colors
#endif

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
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("ClashGrotesk-Light.ttf", "GroteskLight");
                fonts.AddFont("ClashGrotesk-Regular.ttf", "GroteskRegular");
                fonts.AddFont("ClashGrotesk-Semibold.ttf", "GroteskSemibold");
            });
        builder.ConfigureMauiHandlers(handlers =>
        {
#if WINDOWS
            EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                if (handler.PlatformView is TextBox tb)
                {
                    // Убираем рамку/подчёркивание
                    tb.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
                    tb.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 1, 115, 237));
                    tb.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));

                    // На всякий случай глушим стили состояний (PointerOver/Focused)
                    tb.Resources["TextControlBorderBrush"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 1, 115, 237));
                    tb.Resources["TextControlBorderBrushPointerOver"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 1, 115, 237));
                    tb.Resources["TextControlBorderBrushFocused"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 1, 115, 237));
                }
            });
#endif
        });

        LiveCharts.Configure(cfg => cfg.AddSkiaSharp());

        // NEW: профили
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

        // Pages
        builder.Services.AddSingleton<Views.MainPage>();
        builder.Services.AddTransient<Views.RevenuePage>();
        builder.Services.AddTransient<Views.ExpensePage>();
        builder.Services.AddTransient<Views.ProfitPage>();
        builder.Services.AddSingleton<Views.WarehousePage>();

        // NEW: Profile page
        builder.Services.AddSingleton<Views.ProfilePage>();

        return builder.Build();
    }
}
