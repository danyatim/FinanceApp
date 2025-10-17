using CommunityToolkit.Maui.Views;
using FinanceApp.Data;
using FinanceApp.Popups;
using FinanceApp.Services;
using FinanceApp.ViewModels;

namespace FinanceApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfilePageViewModel _vm;
    public ProfilePage(ProfilePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;

        Appearing += async (_, __) => await _vm.ReloadAsync();
    }
}
