using CommunityToolkit.Maui.Views;
using FinanceApp.Data;
using FinanceApp.Popups;
using FinanceApp.Services;

namespace FinanceApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly IProfileService _profiles;
    private readonly IDatabase _db;

    public ProfilePage(IProfileService profiles, IDatabase db)
    {
        InitializeComponent();
        _profiles = profiles;
        _db = db;

        Appearing += async (_, __) => await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        var list = await _profiles.GetProfilesAsync();
        ProfilesCv.ItemsSource = list;
        var cur = _profiles.GetCurrentProfileName() ?? "(не выбран)";
        CurrentLabel.Text = cur;
    }

    private async void OnCreateAndUse(object? sender, EventArgs e)
    {
        var name = NewProfileEntry.Text;
        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Профиль", "Введите имя профиля", "OK");
            return;
        }
        await _profiles.CreateProfileAsync(name);
        await _db.SetProfileAsync(name);
        await DisplayAlert("Профиль", $"Профиль «{name}» выбран.", "OK");

        NewProfileEntry.Text = string.Empty;
        await ReloadAsync();
    }

    private async void OnUseProfile(object? sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is string name)
        {
            await _profiles.SetCurrentProfileAsync(name);
            await _db.SetProfileAsync(name);
            await DisplayAlert("Профиль", $"Профиль «{name}» выбран.", "OK");
            await ReloadAsync();
        }
    }

    private async void OnDeleteProfile(object? sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is string name)
        {
            var confirm = await DisplayAlert("Удалить профиль", $"Удалить профиль «{name}»?", "Да", "Нет");
            if (!confirm) return;

            await _profiles.DeleteProfileAsync(name);
            await ReloadAsync();
        }
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Не используется сейчас; кнопки управляют действиями
    }
    private async void OnButtonSettings(object? sender, EventArgs e)
    {
        var popup = new SettingsPopup();
        await this.ShowPopupAsync(popup);

        //// Возврат на первую вкладку после закрытия
        //if (Shell.Current is Shell shell && shell.Items.FirstOrDefault() is TabBar bar)
        //    shell.CurrentItem = bar.Items.FirstOrDefault();
    }
}
