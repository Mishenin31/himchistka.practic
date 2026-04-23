using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class CartPage : Page
{
    public CartPage(CartPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
