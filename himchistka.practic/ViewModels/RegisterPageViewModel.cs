using System.Windows;
using System.Windows.Input;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class RegisterPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;

    public RegisterPageViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        RegisterCommand = new RelayCommand(Register);
    }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ICommand RegisterCommand { get; }

    private void Register()
    {
        MessageBox.Show("Регистрация успешна");
        _navigationService.Navigate<LoginPage>();
    }
}
