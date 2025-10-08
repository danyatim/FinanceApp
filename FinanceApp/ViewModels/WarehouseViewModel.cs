using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;
using System.Diagnostics;

namespace FinanceApp.ViewModels;

public partial class WarehouseViewModel(IProductService svc, IPopupService popupService) : BaseViewModel
{
    private readonly IProductService _svc = svc;
    private readonly IPopupService _popupService = popupService;

    [ObservableProperty] private bool isArticle = false;

    [ObservableProperty] private List<Product> products = [];
    [ObservableProperty] private string? sortField = "Name";
    [ObservableProperty] private bool sortAscending = true;

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
    public async Task AddSupplyAsync()
    {
        var mainPage = Application.Current?.Windows[0].Page;
        if (mainPage != null)
        {
            var resultObj = await _popupService.ShowPopupAsync<AddSupplyPopupViewModel>();
            if (resultObj is AddSupplyResult result && result.supplyResult != null && result.productsResult != null)
            {
                foreach (var product in result.productsResult)
                {
                    await _svc.AddProductAsync(product);
                }
                await _svc.AddSupplyAsync(result.supplyResult);
            }
            await LoadAsync();
        }
    }

    [RelayCommand]
    public async Task SaveInlineAsync(Product p)
    {
        await _svc.UpdateAsync(p);
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