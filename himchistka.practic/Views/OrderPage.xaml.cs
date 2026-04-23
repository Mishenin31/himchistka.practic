using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class OrderPage : Page
{
    public OrderPage(OrderPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
