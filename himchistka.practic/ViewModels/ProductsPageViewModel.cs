using System.Collections.ObjectModel;
using himchistka.practic.Models;

namespace himchistka.practic.ViewModels;

public class ProductsPageViewModel : BaseViewModel
{
    public ObservableCollection<Product> Products { get; } =
    [
        new() { Id = 1, Name = "Ноутбук Pro", Description = "16GB RAM", Price = 129990, Stock = 8 }
    ];
}
