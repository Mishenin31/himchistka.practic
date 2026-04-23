using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class CheckoutPage : Page
{
    public CheckoutPage(CheckoutPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
