using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;

namespace himchistka.practic.ViewModels;

public class ProductsPageViewModel : BaseViewModel
{
    private readonly CartService _cartService;
    private string _searchText = string.Empty;

    public ProductsPageViewModel(NavigationService navigationService, CartService cartService)
    {
        _cartService = cartService;
        BackCommand = new RelayCommand(() => navigationService.GoBack(), () => navigationService.CanGoBack);
        AddToCartCommand = new RelayCommand(AddToCart);

        Products =
        [
            new() { Id = 1, Name = "Химчистка пальто", Description = "Удаление пятен, отпаривание и восстановление ткани", Price = 3200, Stock = 14, ImagePath = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=800&q=80" },
            new() { Id = 2, Name = "Химчистка костюма", Description = "Деликатная сухая чистка и точечная обработка", Price = 2500, Stock = 20, ImagePath = "https://images.unsplash.com/photo-1594938298603-c8148c4dae35?auto=format&fit=crop&w=800&q=80" },
            new() { Id = 3, Name = "Химчистка пуховика", Description = "Глубокая чистка с восстановлением объема утеплителя", Price = 4200, Stock = 9, ImagePath = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=800&q=80" },
            new() { Id = 4, Name = "Химчистка штор", Description = "Бережная чистка больших текстильных изделий", Price = 5400, Stock = 8, ImagePath = "https://images.unsplash.com/photo-1489515217757-5fd1be406fef?auto=format&fit=crop&w=800&q=80" }
        ];

        FilteredProducts = CollectionViewSource.GetDefaultView(Products);
        FilteredProducts.Filter = item => FilterItem(item as Product);
    }

    public ICommand BackCommand { get; }
    public ICommand AddToCartCommand { get; }

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

    private void AddToCart(object? parameter)
    {
        if (parameter is not Product product)
        {
            return;
        }

        _cartService.Add(product);
        MessageBox.Show($"Услуга «{product.Name}» добавлена в корзину");
    }
}
