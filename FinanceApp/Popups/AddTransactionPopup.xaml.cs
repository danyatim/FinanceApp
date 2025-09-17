using CommunityToolkit.Maui.Views;
using FinanceApp.Models;

namespace FinanceApp.Popups;

public partial class AddTransactionPopup : Popup
{
    public AddTransactionPopup()
    {
        InitializeComponent();
        DatePicker.Date = DateTime.Today;
        DirectionPicker.SelectedIndex = 0;
    }

    private void OnCancel(object? sender, EventArgs e) => Close();

    private void OnSave(object? sender, EventArgs e)
    {
        if (!decimal.TryParse(AmountEntry.Text, out var amount)) return;
        if (DirectionPicker.SelectedItem is not TransactionDirection dir) return;

        var date = DatePicker.Date;
        var t = new Transaction
        {
            Direction = dir,
            Date = date,
            Amount = amount,
            Account = AccountEntry.Text?.Trim() ?? "",
            Source = SourceEntry.Text?.Trim() ?? "",
            Note = NoteEditor.Text
        };
        Close(t);
    }
}