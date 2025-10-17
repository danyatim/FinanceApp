using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data;
using FinanceApp.Models;
using FinanceApp.Services;

namespace FinanceApp.ViewModels
{
    public partial class ProfilePageViewModel(IProfileService profiles, IDatabase db, IPopupService popupService) : ObservableObject
    {
        private readonly IProfileService _profiles = profiles;
        private readonly IDatabase _db = db;
        private readonly IPopupService _popupService = popupService;

        [ObservableProperty] private List<Profile> profilesCv = [];
        [ObservableProperty] private string currentLabel = "";
        [ObservableProperty] private string newProfileEntry = "";
        [ObservableProperty] private Profile? selectedProfile;

        public async Task ReloadAsync()
        {
            ProfilesCv = await _profiles.GetProfilesAsync();
            CurrentLabel = _profiles.GetCurrentProfileName() ?? "(не выбран)";
        }

        [RelayCommand]
        public async Task OnCreateAndUse()
        {
            var mainPage = Application.Current?.Windows[0].Page;
            if (mainPage == null) return;

            if (string.IsNullOrWhiteSpace(NewProfileEntry))
            {
                await mainPage.DisplayAlert("Профиль", "Введите имя профиля", "OK");
                return;
            }
            await _profiles.CreateProfileAsync(NewProfileEntry);
            await _db.SetProfileAsync(NewProfileEntry);
            await mainPage.DisplayAlert("Профиль", $"Профиль «{NewProfileEntry}» выбран.", "OK");

            NewProfileEntry = string.Empty;
            await ReloadAsync();
        }
        [RelayCommand]
        public async Task OnUseProfile()
        {
            if (SelectedProfile == null) return;
            var mainPage = Application.Current?.Windows[0].Page;
            if (mainPage == null || string.IsNullOrWhiteSpace(SelectedProfile.Name)) return;

            await _profiles.SetCurrentProfileAsync(SelectedProfile.Name);
            await _db.SetProfileAsync(SelectedProfile.Name);
            await mainPage.DisplayAlert("Профиль", $"Профиль «{SelectedProfile.Name}» выбран.", "OK");
            await ReloadAsync();
        }
        [RelayCommand]
        public async Task OnDeleteProfile()
        {
            var mainPage = Application.Current?.Windows[0].Page;
            if (mainPage == null) return;
            if (SelectedProfile == null) return;
            var confirm = await mainPage.DisplayAlert("Удалить профиль", $"Удалить профиль «{SelectedProfile.Name}»?", "Да", "Нет");
            if (!confirm) return;

            await _profiles.DeleteProfileAsync(SelectedProfile.Name);
            await ReloadAsync();
        }
        [RelayCommand]
        public async Task OnButtonSettings()
        {
            _ = await _popupService.ShowPopupAsync<SettingsViewModel>();
        }
    }
}
