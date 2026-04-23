using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace himchistka.practic.Models;

public class CartItem : INotifyPropertyChanged
{
    private int _quantity;

    public Product Product { get; set; } = new();

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value)
            {
                return;
            }

            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Total));
        }
    }

    public decimal Total => Product.Price * Quantity;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
