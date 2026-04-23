using System.Windows.Input;
using himchistka.practic.Services;

namespace himchistka.practic.ViewModels;

public class UserProfilePageViewModel : BaseViewModel
{
    private readonly SessionService _sessionService;

    public UserProfilePageViewModel(NavigationService navigationService, SessionService sessionService)
    {
        _sessionService = sessionService;
        BackCommand = new RelayCommand(() => navigationService.GoBack(), () => navigationService.CanGoBack);

        _sessionService.SessionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Phone));
            OnPropertyChanged(nameof(RoleName));
        };
    }

    public ICommand BackCommand { get; }

    public string FullName => _sessionService.CurrentUser?.FullName ?? "Гость";

    public string Email => _sessionService.CurrentUser?.Email ?? "Не указан";

    public string Phone => string.IsNullOrWhiteSpace(_sessionService.CurrentUser?.Phone)
        ? "Не указан"
        : _sessionService.CurrentUser!.Phone;

    public string RoleName => _sessionService.CurrentUser?.IsAdmin == true ? "Администратор" : "Пользователь";
}
