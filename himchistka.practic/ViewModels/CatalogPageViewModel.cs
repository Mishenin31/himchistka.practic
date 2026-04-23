using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class CatalogPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly SessionService _sessionService;
    private readonly CartService _cartService;
    private string _searchText = string.Empty;
    private string _selectedCategory = "Все";

    public CatalogPageViewModel(NavigationService navigationService, SessionService sessionService, CartService cartService)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;
        _cartService = cartService;

        Products = new ObservableCollection<Product>
        {
            new() { Id = 1, Name = "Химчистка пальто", Description = "Удаление пятен, отпаривание и восстановление ткани", Price = 3200, Stock = 14, ImagePath = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=800&q=80" },
            new() { Id = 2, Name = "Химчистка костюма", Description = "Деликатная сухая чистка и точечная обработка", Price = 2500, Stock = 20, ImagePath = "https://images.unsplash.com/photo-1594938298603-c8148c4dae35?auto=format&fit=crop&w=800&q=80" },
            new() { Id = 3, Name = "Химчистка пуховика", Description = "Глубокая чистка с восстановлением объема утеплителя", Price = 4200, Stock = 9, ImagePath = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=800&q=80" },
            new() { Id = 4, Name = "Химчистка штор", Description = "Бережная чистка больших текстильных изделий", Price = 5400, Stock = 8, ImagePath = "https://images.unsplash.com/photo-1489515217757-5fd1be406fef?auto=format&fit=crop&w=800&q=80" },
            new() { Id = 5, Name = "Чистка свадебного платья", Description = "Премиальный уход с ручной обработкой декора", Price = 7600, Stock = 6, ImagePath = "https://images.unsplash.com/photo-1520854221256-17451cc331bf?auto=format&fit=crop&w=800&q=80" },
            new() { Id = 6, Name = "Экспресс-обновление рубашек", Description = "Быстрая чистка и идеальное глажение", Price = 1100, Stock = 30, ImagePath = "https://images.unsplash.com/photo-1602810319428-019690571b5b?auto=format&fit=crop&w=800&q=80" }
        };

        CategoryOptions = ["Все", "До 3000 ₽", "3000–5000 ₽", "От 5000 ₽", "Только в наличии"];

        FilteredProducts = CollectionViewSource.GetDefaultView(Products);
        FilteredProducts.Filter = FilterProduct;

        AddToCartCommand = new RelayCommand(AddToCart);
        OpenCartCommand = new RelayCommand(() => _navigationService.Navigate<CartPage>());
        OpenOrderCommand = new RelayCommand(() => _navigationService.Navigate<OrderPage>());
        OpenUsersCommand = new RelayCommand(OpenAdminCabinet);
        OpenProductsCommand = new RelayCommand(() => _navigationService.Navigate<ProductsPage>());
        LogoutCommand = new RelayCommand(Logout);
    }

    public ObservableCollection<Product> Products { get; }

    public ICollectionView FilteredProducts { get; }

    public IReadOnlyList<string> CategoryOptions { get; }

    public ICommand AddToCartCommand { get; }
    public ICommand OpenCartCommand { get; }
    public ICommand OpenOrderCommand { get; }
    public ICommand OpenUsersCommand { get; }
    public ICommand OpenProductsCommand { get; }
    public ICommand LogoutCommand { get; }

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

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                FilteredProducts.Refresh();
            }
        }
    }

    public string UserName => _sessionService.CurrentUser?.FullName ?? "Гость";

    private bool FilterProduct(object obj)
    {
        if (obj is not Product product)
        {
            return false;
        }

        var hasSearch = string.IsNullOrWhiteSpace(SearchText)
            || product.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)
            || product.Description.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase);

        var hasCategory = SelectedCategory switch
        {
            "До 3000 ₽" => product.Price < 3000,
            "3000–5000 ₽" => product.Price >= 3000 && product.Price <= 5000,
            "От 5000 ₽" => product.Price > 5000,
            "Только в наличии" => product.Stock > 0,
            _ => true
        };

        return hasSearch && hasCategory;
    }

    private void AddToCart(object? parameter)
    {
        if (parameter is Product product)
        {
            _cartService.Add(product);
            MessageBox.Show($"Услуга «{product.Name}» добавлена в корзину");
        }
    }

    private void OpenAdminCabinet()
    {
        if (_sessionService.CurrentUser?.IsAdmin != true)
        {
            MessageBox.Show("Личный кабинет администратора доступен только администратору.");
            return;
        }

        _navigationService.Navigate<UsersPage>();
    }

    private void Logout()
    {
        _sessionService.Logout();
        _navigationService.Navigate<LoginPage>();
    }
}
