using System.Windows;
using System.Windows.Input;
using himchistka.practic.Services;
using himchistka.practic.Views;
using QRCoder;

namespace himchistka.practic.ViewModels;

public class CheckoutPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;

    public CheckoutPageViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        BackCommand = new RelayCommand(() => _navigationService.GoBack(), () => _navigationService.CanGoBack);
        PlaceOrderCommand = new RelayCommand(PlaceOrder);
    }

    public string Address { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "Карта";
    public string Comment { get; set; } = string.Empty;

    public ICommand BackCommand { get; }
    public ICommand PlaceOrderCommand { get; }

    private void PlaceOrder()
    {
        var orderInfo = BuildOrderInfo();
        var qrCodeBytes = GenerateQrCode(orderInfo);

        var qrWindow = new OrderQrWindow(qrCodeBytes);
        qrWindow.ShowDialog();

        MessageBox.Show("Заказ оформлен");
        _navigationService.Navigate<OrderPage>();
    }

    private string BuildOrderInfo()
    {
        return $"Номер: {Guid.NewGuid():N}\n" +
               $"Дата: {DateTime.Now:dd.MM.yyyy HH:mm}\n" +
               $"Адрес: {Address}\n" +
               $"Оплата: {PaymentMethod}\n" +
               $"Комментарий: {Comment}";
    }

    private static byte[] GenerateQrCode(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var pngQrCode = new PngByteQRCode(data);
        return pngQrCode.GetGraphic(20);
    }
}
