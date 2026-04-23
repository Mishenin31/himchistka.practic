using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class CartPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;

    public CartPageViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        Items = new ObservableCollection<CartItem>();
        BackCommand = new RelayCommand(() => _navigationService.GoBack(), () => _navigationService.CanGoBack);
        CheckoutCommand = new RelayCommand(() => _navigationService.Navigate<CheckoutPage>());
    }

    public ObservableCollection<CartItem> Items { get; }
    public decimal Total => Items.Sum(x => x.Total);
    public ICommand BackCommand { get; }
    public ICommand CheckoutCommand { get; }
}
