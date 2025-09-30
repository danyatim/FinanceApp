using CommunityToolkit.Maui.Views;
using FinanceApp.Models;
using FinanceApp.Services;

namespace FinanceApp.Popups;

public partial class AddTransactionPopup : Popup
{
    private readonly IReferenceService _refs;

    public AddTransactionPopup(IReferenceService refs)
    {
        InitializeComponent();
        _refs = refs;

        DatePicker.Date = DateTime.Today;
        DirectionPicker.SelectedIndex = 0;

        DirectionPicker.SelectedIndexChanged += async (_, __) => await LoadSourcesAsync();

        // «агружаем справочники, когда попап открылс€
        this.Opened += async (_, __) =>
        {
            await LoadAccountsAsync();
            await LoadSourcesAsync();
        };
    }

    private async Task LoadAccountsAsync()
    {
        var accounts = await _refs.GetAccountsAsync();
        AccountPicker.ItemsSource = accounts.Select(a => a.Name).ToList();
    }

    private async Task LoadSourcesAsync()
    {
        TransactionDirection? type = null;
        if (DirectionPicker.SelectedItem is TransactionDirection td) type = td;
        var sources = await _refs.GetSourcesAsync(type);
        SourcePicker.ItemsSource = sources.Select(s => s.Name).ToList();
    }

    private void OnCancel(object? sender, EventArgs e) => Close();

    private void OnSave(object? sender, EventArgs e)
    {
        if (!decimal.TryParse(AmountEntry.Text, out var amount)) return;
        if (DirectionPicker.SelectedItem is not TransactionDirection dir) return;

        var account = AccountPicker.SelectedItem as string;
        var source = SourcePicker.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(source)) return;

        var t = new Transaction
        {
            Direction = dir,
            Date = DatePicker.Date,
            Amount = amount,
            Account = account,
            Source = source,
            Note = NoteEditor.Text
        };
        Close(t);
    }

    private void AmountEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        AmountEntry.Text = AmountEntry.Text.Replace(".", ",");
    }
}
