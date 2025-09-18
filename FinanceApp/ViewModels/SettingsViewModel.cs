using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;
using System.Collections.ObjectModel;

namespace FinanceApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IReferenceService _refs;

    [ObservableProperty] private string? newAccountName;
    [ObservableProperty] private string? newSourceName;
    [ObservableProperty] private TransactionDirection newSourceType = TransactionDirection.Expense;

    public ObservableCollection<Account> Accounts { get; } = new();
    public ObservableCollection<Source> Sources { get; } = new();

    public IEnumerable<TransactionDirection> SourceTypes { get; } =
        Enum.GetValues(typeof(TransactionDirection)).Cast<TransactionDirection>();

    public Action? CloseRequested { get; set; }

    public SettingsViewModel(IReferenceService refs)
    {
        _refs = refs;
    }

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
    private async Task DeleteAccountAsync(int id)
    {
        await _refs.DeleteAccountAsync(id);
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
    private async Task DeleteSourceAsync(int id)
    {
        await _refs.DeleteSourceAsync(id);
        await LoadAsync();
    }

    [RelayCommand]
    private void Close() => CloseRequested?.Invoke();
}
