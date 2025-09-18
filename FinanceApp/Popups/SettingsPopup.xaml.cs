using CommunityToolkit.Maui.Views;
using FinanceApp.ViewModels;

namespace FinanceApp.Popups;

public partial class SettingsPopup : Popup
{
    public SettingsPopup()
    {
        InitializeComponent();
        this.Opened += SettingsPopup_Opened;
    }

    private async void SettingsPopup_Opened(object? sender, EventArgs e)
    {
        var services = this.Handler?.MauiContext?.Services;
        if (services == null) return;

        var vm = services.GetService(typeof(SettingsViewModel)) as SettingsViewModel;
        if (vm == null) return;

        vm.CloseRequested = () => Close();
        BindingContext = vm;
        await vm.LoadAsync();
    }
}
