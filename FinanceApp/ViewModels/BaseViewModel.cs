using CommunityToolkit.Mvvm.ComponentModel;

namespace FinanceApp.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? title;
}