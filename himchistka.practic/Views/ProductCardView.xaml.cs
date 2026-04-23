using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace himchistka.practic.Views;

public partial class ProductCardView : UserControl
{
    public static readonly DependencyProperty AddToCartCommandProperty =
        DependencyProperty.Register(nameof(AddToCartCommand), typeof(ICommand), typeof(ProductCardView), new PropertyMetadata(null));

    public ProductCardView()
    {
        InitializeComponent();
    }

    public ICommand? AddToCartCommand
    {
        get => (ICommand?)GetValue(AddToCartCommandProperty);
        set => SetValue(AddToCartCommandProperty, value);
    }
}
