using CommunityToolkit.Maui;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Maui;
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
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("ClashGrotesk-Light.ttf", "GroteskLight");
                fonts.AddFont("ClashGrotesk-Regular.ttf", "GroteskRegular");
                fonts.AddFont("ClashGrotesk-Semibold.ttf", "GroteskSemibold");
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
