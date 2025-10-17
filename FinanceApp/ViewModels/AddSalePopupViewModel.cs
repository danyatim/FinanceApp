using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FinanceApp.ViewModels
{
    public partial class AddSalePopupViewModel(IReferenceService referenceService): ObservableObject
    {
        public event EventHandler<SaleResult>? RequestClose;
        private readonly IReferenceService _refs = referenceService;


        [ObservableProperty] private ProductAndSupply? productAndSupply;
        [ObservableProperty] private DateTime date = DateTime.Today;
        [ObservableProperty] private Product? product;
        [ObservableProperty] private Supply? supply;

        [ObservableProperty] private List<string> sources = new();

        [ObservableProperty] private decimal amountExpenses;
        [ObservableProperty] private string? selectedSource;
        [ObservableProperty] private string? selectedAccount;

        [ObservableProperty] private List<string> accounts = new();

        [ObservableProperty] private int quantity ;
        [ObservableProperty] private decimal price ;

        [ObservableProperty] private ObservableCollection<ExpensesItem> expenses = new();

        public async Task Load()
        {
            var sources = await _refs.GetSourcesAsync(TransactionDirection.Expense);
            Sources = sources.Select(s => s.Name).ToList();
            var accounts = await _refs.GetAccountsAsync();
            Accounts = accounts.Select(s => s.Name).ToList();
            Product = ProductAndSupply?.ProductMessage;
            Supply = ProductAndSupply?.SupplyMessage;
        }

        [RelayCommand]
        private void OnAddExpenses()
        {
            if (SelectedSource == null) return;
            _ = decimal.TryParse(AmountExpenses.ToString(), out var expenses);
            Expenses.Add(new ExpensesItem()
            {
                SourceExpenses = SelectedSource,
                Expense = expenses
            });
        }

        [RelayCommand]
        private void OnSave()
        {
            if (Quantity <= 0 || Product == null) return;
            _ = int.TryParse(Quantity.ToString(), out var quantity);
            _ = decimal.TryParse(Price.ToString(), out var priceResult);

            decimal PriceDelivery = 0m;
            if (Supply != null)
            {
                PriceDelivery = Math.Round((Supply.DeliveryPrice / Supply.CountProduct) * quantity, 2);
                Debug.WriteLine(PriceDelivery);
                Product.DeliveryPrice -= PriceDelivery;
            }
            Product.Quantity -= quantity;
            Expenses.Add(new ExpensesItem
            {
                Expense = Product.BuyPrice,
                SourceExpenses = "Покупка товара"
            });
            Expenses.Add(new ExpensesItem
            {
                Expense = PriceDelivery,
                SourceExpenses = "Доставка товара"
            });
            
            RequestClose?.Invoke(this, new SaleResult
            {
                ProductSale = Product,
                Account = SelectedAccount,
                Date = Date,
                Income = priceResult,
                Quantity = quantity,
                Expenses = Expenses.ToList(),
            });
        }
        [RelayCommand]
        private void OnCancel()
        {
            RequestClose?.Invoke(this, new SaleResult { ProductSale = null });
        }
    }
}
