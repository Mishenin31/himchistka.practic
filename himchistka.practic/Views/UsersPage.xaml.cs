using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class UsersPage : Page
{
    public UsersPage(UsersPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
