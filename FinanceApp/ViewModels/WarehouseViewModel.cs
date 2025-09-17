using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Models;
using FinanceApp.Services;

namespace FinanceApp.ViewModels;

public partial class WarehouseViewModel : BaseViewModel
{
    private readonly IProductService _svc;

    [ObservableProperty] private List<Product> products = new();
    [ObservableProperty] private string? sortField = "Name";
    [ObservableProperty] private bool sortAscending = true;

    public WarehouseViewModel(IProductService svc)
    {
        _svc = svc;
        Title = "Склад";
    }

    [RelayCommand]
    public async Task LoadAsync() => Products = await _svc.GetAllAsync(SortField, SortAscending);

    [RelayCommand]
    public async Task ToggleSortAsync(string field)
    {
        if (SortField?.Equals(field, StringComparison.OrdinalIgnoreCase) == true)
            SortAscending = !SortAscending;
        else
        {
            SortField = field;
            SortAscending = true;
        }
        await LoadAsync();
    }

    [RelayCommand]
    public async Task AddProductAsync()
    {
        var mainPage = Application.Current?.Windows[0].Page;
        if (mainPage != null)
        {
            var popup = new Popups.AddProductPopup();
            var result = await mainPage.ShowPopupAsync(popup);
            if (result is Product p)
            {
                await _svc.AddAsync(p);
                await LoadAsync();
            }
        }
        
    }

    [RelayCommand]
    public async Task SaveInlineAsync(Product p)
    {
        await _svc.UpdateAsync(p);
        await LoadAsync();
    }
}