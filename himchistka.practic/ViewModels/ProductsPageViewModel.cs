using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;

namespace himchistka.practic.ViewModels;

public class ProductsPageViewModel : BaseViewModel
{
    private string _searchText = string.Empty;

    public ProductsPageViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.GoBack(), () => navigationService.CanGoBack);

        Products =
        [
            new() { Id = 1, Name = "Химчистка пальто", Description = "Удаление пятен, отпаривание и восстановление ткани", Price = 3200, Stock = 14 },
            new() { Id = 2, Name = "Химчистка костюма", Description = "Деликатная сухая чистка и точечная обработка", Price = 2500, Stock = 20 },
            new() { Id = 3, Name = "Химчистка пуховика", Description = "Глубокая чистка с восстановлением объема утеплителя", Price = 4200, Stock = 9 },
            new() { Id = 4, Name = "Химчистка штор", Description = "Бережная чистка больших текстильных изделий", Price = 5400, Stock = 8 }
        ];

        FilteredProducts = CollectionViewSource.GetDefaultView(Products);
        FilteredProducts.Filter = item => FilterItem(item as Product);
    }

    public ICommand BackCommand { get; }

    public ObservableCollection<Product> Products { get; }

    public ICollectionView FilteredProducts { get; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilteredProducts.Refresh();
            }
        }
    }

    private bool FilterItem(Product? product)
    {
        if (product is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return true;
        }

        return product.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)
               || product.Description.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase);
    }
}
