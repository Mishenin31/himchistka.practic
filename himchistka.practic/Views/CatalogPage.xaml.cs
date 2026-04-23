using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class CatalogPage : Page
{
    public CatalogPage(CatalogPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
