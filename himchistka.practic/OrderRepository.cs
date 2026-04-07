using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace himchistka.practic
{
    public sealed class OrderRepository
    {
        private int _nextId = 6;

        public ObservableCollection<OrderRecord> Orders { get; } = new ObservableCollection<OrderRecord>
        {
            new OrderRecord { Id = 1, ClientFullName = "Иванов И.И.", ServiceName = "Химчистка пальто", DateReceived = DateTime.Today.AddDays(-2), TotalPrice = 2500, Status = "В работе" },
            new OrderRecord { Id = 2, ClientFullName = "Петрова А.А.", ServiceName = "Чистка ковра", DateReceived = DateTime.Today.AddDays(-5), TotalPrice = 3200, Status = "Готов" },
            new OrderRecord { Id = 3, ClientFullName = "Сидоров П.П.", ServiceName = "Удаление пятен", DateReceived = DateTime.Today.AddDays(-1), TotalPrice = 1800, Status = "Новый" },
            new OrderRecord { Id = 4, ClientFullName = "Васильева М.С.", ServiceName = "Химчистка платья", DateReceived = DateTime.Today.AddDays(-4), TotalPrice = 2700, Status = "Выдан" },
            new OrderRecord { Id = 5, ClientFullName = "Морозов Е.В.", ServiceName = "Чистка костюма", DateReceived = DateTime.Today.AddDays(-3), TotalPrice = 2300, Status = "В работе" }
        };

        public ObservableCollection<ClientRecord> Clients { get; } = new ObservableCollection<ClientRecord>
        {
            new ClientRecord { Id = 1, FullName = "Иванов И.И.", Phone = "+7 (900) 123-45-67", LoyaltyLevel = "Серебро" },
            new ClientRecord { Id = 2, FullName = "Петрова А.А.", Phone = "+7 (911) 555-66-77", LoyaltyLevel = "Золото" },
            new ClientRecord { Id = 3, FullName = "Сидоров П.П.", Phone = "+7 (999) 888-77-66", LoyaltyLevel = "Стандарт" }
        };

        public OrderRecord AddOrder(OrderRecord source)
        {
            var order = new OrderRecord
            {
                Id = _nextId++,
                ClientFullName = source.ClientFullName,
                ServiceName = source.ServiceName,
                DateReceived = source.DateReceived,
                TotalPrice = source.TotalPrice,
                Status = source.Status
            };

            Orders.Add(order);
            return order;
        }

        public void DeleteOrder(int id)
        {
            var order = Orders.FirstOrDefault(x => x.Id == id);
            if (order != null)
            {
                Orders.Remove(order);
            }
        }
    }
}
