using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace himchistka.practic.Views;

public partial class OrderQrWindow : Window
{
    public OrderQrWindow(byte[] qrCodePng)
    {
        InitializeComponent();

        var image = new BitmapImage();
        using var stream = new MemoryStream(qrCodePng);
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();

        QrImage.Source = image;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
