using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class CatalogPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly SessionService _sessionService;

    public CatalogPageViewModel(NavigationService navigationService, SessionService sessionService)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;

        Products = new ObservableCollection<Product>
        {
            new() { Id = 1, Name = "Ноутбук Pro", Description = "16GB RAM, 512GB SSD", Price = 129990, Stock = 8 },
            new() { Id = 2, Name = "Смартфон X", Description = "AMOLED, 256GB", Price = 79990, Stock = 14 },
            new() { Id = 3, Name = "Наушники ANC", Description = "Шумоподавление", Price = 19990, Stock = 20 }
        };

        AddToCartCommand = new RelayCommand(AddToCart);
        OpenCartCommand = new RelayCommand(() => _navigationService.Navigate<CartPage>());
        OpenOrderCommand = new RelayCommand(() => _navigationService.Navigate<OrderPage>());
        OpenUsersCommand = new RelayCommand(() => _navigationService.Navigate<UsersPage>());
        OpenProductsCommand = new RelayCommand(() => _navigationService.Navigate<ProductsPage>());
        LogoutCommand = new RelayCommand(Logout);
    }

    public ObservableCollection<Product> Products { get; }

    public ICommand AddToCartCommand { get; }
    public ICommand OpenCartCommand { get; }
    public ICommand OpenOrderCommand { get; }
    public ICommand OpenUsersCommand { get; }
    public ICommand OpenProductsCommand { get; }
    public ICommand LogoutCommand { get; }

    public string UserName => _sessionService.CurrentUser?.FullName ?? "Гость";

    public bool IsAdmin => _sessionService.CurrentUser?.IsAdmin == true;

    private void AddToCart(object? parameter)
    {
        if (parameter is Product product)
        {
            MessageBox.Show($"{product.Name} добавлен в корзину");
        }
    }

    private void Logout()
    {
        _sessionService.Logout();
        _navigationService.Navigate<LoginPage>();
    }
}
