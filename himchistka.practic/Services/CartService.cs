using System;
using System.Collections.ObjectModel;
using System.Linq;
using himchistka.practic.Models;

namespace himchistka.practic.Services;

public class CartService
{
    public ObservableCollection<CartItem> Items { get; } = [];

    public event EventHandler? CartChanged;

    public decimal Total => Items.Sum(x => x.Total);

    public void Add(Product product)
    {
        var existing = Items.FirstOrDefault(x => x.Product.Id == product.Id);
        if (existing is null)
        {
            Items.Add(new CartItem { Product = product, Quantity = 1 });
        }
        else
        {
            existing.Quantity++;
        }

        CartChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Remove(CartItem item)
    {
        if (Items.Remove(item))
        {
            CartChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Clear()
    {
        Items.Clear();
        CartChanged?.Invoke(this, EventArgs.Empty);
    }
}
