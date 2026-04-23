using System.Collections.ObjectModel;
using himchistka.practic.Models;

namespace himchistka.practic.ViewModels;

public class UsersPageViewModel : BaseViewModel
{
    public ObservableCollection<User> Users { get; } =
    [
        new() { Id = 1, FullName = "Администратор", Email = "admin@shop.local", IsAdmin = true },
        new() { Id = 2, FullName = "Иван Иванов", Email = "user@shop.local", IsAdmin = false }
    ];
}
