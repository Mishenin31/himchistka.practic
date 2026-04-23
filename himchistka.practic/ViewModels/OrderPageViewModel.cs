using System.Windows.Input;
using himchistka.practic.Services;

namespace himchistka.practic.ViewModels;

public class OrderPageViewModel : BaseViewModel
{
    public OrderPageViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.GoBack(), () => navigationService.CanGoBack);
    }

    public ICommand BackCommand { get; }
    public string Status { get; } = "В обработке";
    public string CustomerInfo { get; } = "Иван Иванов, +7 (999) 000-00-00";
    public string Composition { get; } = "Пальто шерстяное x1, Пуховик зимний x1";
}
