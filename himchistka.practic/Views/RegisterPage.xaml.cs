using System.Windows.Controls;
using himchistka.practic.ViewModels;

namespace himchistka.practic.Views;

public partial class RegisterPage : Page
{
    public RegisterPage(RegisterPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
