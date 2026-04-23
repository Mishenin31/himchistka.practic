using System.Windows;
using System.Windows.Input;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class CheckoutPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;

    public CheckoutPageViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        BackCommand = new RelayCommand(() => _navigationService.GoBack(), () => _navigationService.CanGoBack);
        PlaceOrderCommand = new RelayCommand(PlaceOrder);
    }

    public string Address { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "Карта";
    public string Comment { get; set; } = string.Empty;

    public ICommand BackCommand { get; }
    public ICommand PlaceOrderCommand { get; }

    private void PlaceOrder()
    {
        MessageBox.Show("Заказ оформлен");
        _navigationService.Navigate<OrderPage>();
    }
}
