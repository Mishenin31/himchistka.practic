using System.Collections.ObjectModel;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;

namespace himchistka.practic.ViewModels;

public class ProductsPageViewModel : BaseViewModel
{
    public ProductsPageViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.GoBack(), () => navigationService.CanGoBack);
    }

    public ICommand BackCommand { get; }

    public ObservableCollection<Product> Products { get; } =
    [
        new() { Id = 1, Name = "Химчистка костюма", Description = "Деликатная сухая чистка и отпаривание", Price = 2500, Stock = 16 }
    ];
}
