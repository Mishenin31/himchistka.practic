using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class UserProfilePage : Page
{
    public UserProfilePage(UserProfilePageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
