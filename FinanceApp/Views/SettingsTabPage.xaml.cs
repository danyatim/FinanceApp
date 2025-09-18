using CommunityToolkit.Maui.Views;
using FinanceApp.Popups;

namespace FinanceApp.Views;

public partial class SettingsTabPage : ContentPage
{
    private bool _opened;

    public SettingsTabPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_opened) return;
        _opened = true;

        var popup = new SettingsPopup();
        await this.ShowPopupAsync(popup);

        // ¬озврат на первую вкладку после закрыти€
        if (Shell.Current is Shell shell && shell.Items.FirstOrDefault() is TabBar bar)
            shell.CurrentItem = bar.Items.FirstOrDefault();

        _opened = false;
    }
}
