using CommunityToolkit.Maui.Views;
using FinanceApp.ViewModels;

namespace FinanceApp.Popups;

public partial class SettingsPopup : Popup
{
    public SettingsPopup(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        BindingContext = settingsViewModel;
        this.Opened += SettingsPopup_Opened;
    }

    private async void SettingsPopup_Opened(object? sender, EventArgs e)
    {
        var services = this.Handler?.MauiContext?.Services;
        if (services == null) return;

        if (services.GetService(typeof(SettingsViewModel)) is not SettingsViewModel vm) return;


        vm.CloseRequested = () => Close();
        BindingContext = vm;
        await vm.LoadAsync();
    }
}
