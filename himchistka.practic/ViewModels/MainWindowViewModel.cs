using System.Windows.Input;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly SessionService _sessionService;
    private readonly NavigationService _navigationService;

    public MainWindowViewModel(SessionService sessionService, NavigationService navigationService)
    {
        _sessionService = sessionService;
        _navigationService = navigationService;

        LogoutCommand = new RelayCommand(Logout, () => _sessionService.IsAuthenticated);

        _sessionService.SessionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(CurrentUserName));
            (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
        };
    }

    public string HeaderTitle => "Система заказов";

    public string CurrentUserName => _sessionService.CurrentUser?.FullName ?? "Гость";

    public ICommand LogoutCommand { get; }

    private void Logout()
    {
        _sessionService.Logout();
        _navigationService.Navigate<LoginPage>();
    }
}
