using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class ProductsPage : Page
{
    public ProductsPage(ProductsPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
