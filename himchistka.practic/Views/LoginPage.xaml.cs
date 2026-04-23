using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class LoginPage : Page
{
    public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
