using System;
using System.Collections.Generic;

namespace himchistka.practic.Models;

public class Order
{
    public int Id { get; set; }
    public User Customer { get; set; } = new();
    public List<CartItem> Items { get; set; } = [];
    public string Address { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string Status { get; set; } = "Новый";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }
}
