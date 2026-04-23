using System.Windows;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class LoginPageViewModel : BaseViewModel
{
    private readonly SessionService _sessionService;
    private readonly NavigationService _navigationService;

    private string _email = string.Empty;
    private string _password = string.Empty;

    public LoginPageViewModel(SessionService sessionService, NavigationService navigationService)
    {
        _sessionService = sessionService;
        _navigationService = navigationService;
        LoginCommand = new RelayCommand(Login);
        GoToRegisterCommand = new RelayCommand(() => _navigationService.Navigate<RegisterPage>());
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    private void Login()
    {
        var isAdmin = Email.Contains("admin");
        var user = new User
        {
            Id = 1,
            FullName = isAdmin ? "Администратор" : "Иван Иванов",
            Email = Email,
            Phone = isAdmin ? "+7 (700) 000-00-01" : "+7 (700) 000-00-02",
            IsAdmin = isAdmin
        };

        _sessionService.Login(user);
        _navigationService.Navigate<CatalogPage>();
        MessageBox.Show("Вход выполнен");
    }
}
