using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FinanceApp.ViewModels;

public partial class SettingsViewModel(IReferenceService refs, IPopupService popupService) : ObservableObject
{
    private readonly IReferenceService _refs = refs;
    private readonly IPopupService _popupService = popupService;

    [ObservableProperty] private string? newAccountName;
    [ObservableProperty] private string? newSourceName;
    [ObservableProperty] private Account? selectedAccount;
    [ObservableProperty] private Source? selectedSource;
    [ObservableProperty] private TransactionDirection newSourceType = TransactionDirection.Expense;

    public ObservableCollection<Account> Accounts { get; } = [];
    public ObservableCollection<Source> Sources { get; } = [];


    public IEnumerable<TransactionDirection> SourceTypes { get; } =
        Enum.GetValues(typeof(TransactionDirection)).Cast<TransactionDirection>();

    public Action? CloseRequested { get; set; }

    public async Task LoadAsync()
    {
        Accounts.Clear();
        foreach (var a in await _refs.GetAccountsAsync()) Accounts.Add(a);

        Sources.Clear();
        foreach (var s in await _refs.GetSourcesAsync()) Sources.Add(s);
    }

    [RelayCommand]
    private async Task AddAccountAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewAccountName))
        {
            await _refs.AddAccountAsync(NewAccountName!);
            NewAccountName = string.Empty;
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        if (SelectedAccount == null) return;
        await _refs.DeleteAccountAsync(SelectedAccount.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task AddSourceAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewSourceName))
        {
            await _refs.AddSourceAsync(NewSourceName!, NewSourceType);
            NewSourceName = string.Empty;
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteSourceAsync()
    {
        if (SelectedSource == null) return;
        await _refs.DeleteSourceAsync(SelectedSource.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private void Close() => CloseRequested?.Invoke();
}
