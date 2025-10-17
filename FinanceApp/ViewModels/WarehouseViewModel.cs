using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;
using System.Diagnostics;

namespace FinanceApp.ViewModels;

public partial class WarehouseViewModel(IProductService svc, IPopupService popupService, ITransactionService tx) : BaseViewModel
{
    private readonly ITransactionService _tx = tx;
    private readonly IProductService _svc = svc;
    private readonly IPopupService _popupService = popupService;

    [ObservableProperty] private List<Product> products = [];
    [ObservableProperty] private string? sortField = "Name";
    [ObservableProperty] private bool sortAscending = true;
    [ObservableProperty] private Product? selectedProduct;

    [RelayCommand]
    public async Task LoadAsync() => Products = await _svc.GetAllAsync(SortField, SortAscending);

    [RelayCommand]
    public async Task ToggleSortAsync(string? field)
    {
        if (SortField?.Equals(field, StringComparison.OrdinalIgnoreCase) == true)
            SortAscending = !SortAscending;
        else
        {
            SortField = field;
            SortAscending = !SortAscending;
        }
        await LoadAsync();
    }

    [RelayCommand]
    public async Task SaleProduct()
    {
        if (SelectedProduct == null) return;
        Supply supply = await _svc.GetSupplyAsync(SelectedProduct.SupplyId);
        var message = new ProductAndSupply
        {
            ProductMessage = SelectedProduct,
            SupplyMessage = supply
        };
        Debug.WriteLine($"SelectedProduct.SupplyId = {SelectedProduct.SupplyId}");
        var saleObj = await _popupService.ShowPopupAsync<AddSalePopupViewModel>(onPresenting: viewModel => viewModel.ProductAndSupply = message);
        if (saleObj is SaleResult result && result.ProductSale != null)
        {
            if (result.ProductSale.Quantity <= 0) await _svc.DeleteAsync(SelectedProduct);

            SelectedProduct = result.ProductSale;
            await SaveInlineAsync();

            Transaction newTrans = new()
            {
                Account = string.IsNullOrEmpty(result.Account) ? "" : result.Account,
                Date = result.Date,
                Amount = result.Income,
                Direction = result.Income >= 0 ? TransactionDirection.Income : TransactionDirection.Expense,
                Source = "Продажа со склада",
            };
            await _tx.AddAsync(newTrans);

            if (result.Expenses != null)
            {
                foreach (ExpensesItem exp in result.Expenses)
                {
                    Transaction transaction = new()
                    {
                        Account = string.IsNullOrEmpty(result.Account) ? "" : result.Account,
                        Date = result.Date,
                        Amount = exp.Expense,
                        Source = exp.SourceExpenses,
                        Direction = TransactionDirection.Expense
                    };
                    await _tx.AddAsync(transaction);
                }
            }
            
        }
    }

    [RelayCommand]
    public async Task AddSupplyAsync()
    {
        var mainPage = Application.Current?.Windows[0].Page;
        if (mainPage != null)
        {
            var resultObj = await _popupService.ShowPopupAsync<AddSupplyPopupViewModel>();
            if (resultObj is AddSupplyResult result && result.SupplyResult != null && result.ProductsResult != null)
            {
                await _svc.AddSupplyAsync(result.SupplyResult);

                foreach (var product in result.ProductsResult)
                {
                    product.SupplyId = result.SupplyResult.Id;
                    await _svc.AddProductAsync(product);
                }
            }
            await LoadAsync();
        }
    }

    [RelayCommand]
    public async Task SaveInlineAsync()
    {
        if (SelectedProduct == null) return;
        await _svc.UpdateAsync(SelectedProduct);
        await LoadAsync();
    }

    [RelayCommand]
    public async Task ImportXlsxAsync()
    {
        var XlsxFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".xlsx" } }, // file extension
                });
        var options = new PickOptions { FileTypes = XlsxFileType };

        var fileResult = await FilePicker.PickAsync(options);

        var filePath = fileResult?.FullPath;
        if (filePath == null) return;

        var prices = OzonXlsxParserService.ParsePrices(filePath);
        foreach (var p in prices)
        {
            for (int i = 0; i <= Products.Count-1; i++)
            {
                if (Products[i].Article != p.Article) continue;
                var product = Products[i];
                product.SellPrice = p.Price;
                product.FeePercent = p.OzonRewardFBS;
                product.OzonExpensesSum = p.ExpensesSum;
                product.OzonPercent = p.OzonRewardFBS;
                await _svc.UpdateAsync(product);
            }
        }
        await LoadAsync();
    }
}