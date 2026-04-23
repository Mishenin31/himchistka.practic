namespace himchistka.practic.ViewModels;

public class OrderPageViewModel : BaseViewModel
{
    public string Status { get; } = "В обработке";
    public string CustomerInfo { get; } = "Иван Иванов, +7 (999) 000-00-00";
    public string Composition { get; } = "Ноутбук Pro x1, Наушники ANC x1";
}
