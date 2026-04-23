using System.Collections.ObjectModel;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;

namespace himchistka.practic.ViewModels;

public class UsersPageViewModel : BaseViewModel
{
    public UsersPageViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.GoBack(), () => navigationService.CanGoBack);
    }

    public ICommand BackCommand { get; }

    public ObservableCollection<User> Users { get; } =
    [
        new() { Id = 1, FullName = "Администратор", Email = "admin@clean.local", IsAdmin = true },
        new() { Id = 2, FullName = "Иван Иванов", Email = "client@clean.local", IsAdmin = false }
    ];
}
