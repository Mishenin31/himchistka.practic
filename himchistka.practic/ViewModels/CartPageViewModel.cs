using System.Collections.ObjectModel;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class CartPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly CartService _cartService;

    public CartPageViewModel(NavigationService navigationService, CartService cartService)
    {
        _navigationService = navigationService;
        _cartService = cartService;
        Items = _cartService.Items;

        BackCommand = new RelayCommand(() => _navigationService.GoBack(), () => _navigationService.CanGoBack);
        CheckoutCommand = new RelayCommand(() => _navigationService.Navigate<CheckoutPage>(), () => Items.Count > 0);
        RemoveItemCommand = new RelayCommand(RemoveItem);

        _cartService.CartChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(Total));
            (CheckoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
        };
    }

    public ObservableCollection<CartItem> Items { get; }
    public decimal Total => _cartService.Total;
    public ICommand BackCommand { get; }
    public ICommand CheckoutCommand { get; }
    public ICommand RemoveItemCommand { get; }

    private void RemoveItem(object? parameter)
    {
        if (parameter is CartItem item)
        {
            _cartService.Remove(item);
        }
    }
}
