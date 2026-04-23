using System.Windows;
using himchistka.practic.Services;
using himchistka.practic.ViewModels;

namespace himchistka.practic;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel, NavigationService navigationService)
    {
        InitializeComponent();
        DataContext = viewModel;
        navigationService.SetFrame(MainFrame);
    }
}
