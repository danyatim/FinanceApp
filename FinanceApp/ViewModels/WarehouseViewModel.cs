using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;

namespace FinanceApp.ViewModels;

public partial class WarehouseViewModel : BaseViewModel
{
    private readonly IProductService _svc;
    private readonly IPopupService _popupService;

    [ObservableProperty] private List<Product> products = new();
    [ObservableProperty] private string? sortField = "Name";
    [ObservableProperty] private bool sortAscending = true;

    public WarehouseViewModel(IProductService svc, IPopupService popupService)
    {
        _svc = svc;
        _popupService = popupService;
    }

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
}