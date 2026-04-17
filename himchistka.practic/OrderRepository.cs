using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace himchistka.practic
{
    public sealed class OrderRepository
    {
        private int _nextId = 6;
        private int _nextCartId = 1;
        private int _nextReferenceId = 4;

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

        public ObservableCollection<ServiceCatalogRecord> ServicesCatalog { get; } = new ObservableCollection<ServiceCatalogRecord>
        {
            new ServiceCatalogRecord { Id = 1, Name = "Химчистка пальто", Category = "Верхняя одежда", Duration = "2-3 дня", BasePrice = 2500, IsActive = true },
            new ServiceCatalogRecord { Id = 2, Name = "Химчистка платья", Category = "Одежда", Duration = "1-2 дня", BasePrice = 2700, IsActive = true },
            new ServiceCatalogRecord { Id = 3, Name = "Чистка костюма", Category = "Одежда", Duration = "1-2 дня", BasePrice = 2300, IsActive = true },
            new ServiceCatalogRecord { Id = 4, Name = "Чистка ковра", Category = "Домашний текстиль", Duration = "3-5 дней", BasePrice = 3200, IsActive = true },
            new ServiceCatalogRecord { Id = 5, Name = "Удаление пятен", Category = "Дополнительная услуга", Duration = "до 1 дня", BasePrice = 1800, IsActive = true }
        };

        public ObservableCollection<CartItem> Cart { get; } = new ObservableCollection<CartItem>();

        public ObservableCollection<ReferenceRecord> References { get; } = new ObservableCollection<ReferenceRecord>
        {
            new ReferenceRecord { Id = 1, Type = "Страна", Name = "Россия", Value = "RU" },
            new ReferenceRecord { Id = 2, Type = "Город", Name = "Москва", Value = "MSK" },
            new ReferenceRecord { Id = 3, Type = "Статус", Name = "Принят", Value = "OrderStatus" }
        };

        public OrderRecord AddOrder(OrderRecord source)
        {
            if (!ValidationService.IsValidOrder(source))
            {
                throw new InvalidOperationException("Некорректные данные заказа.");
            }

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

        public OrderRecord CreateOrderFromCart(string clientFullName)
        {
            if (Cart.Count == 0)
            {
                throw new InvalidOperationException("Корзина пуста.");
            }

            var order = AddOrder(new OrderRecord
            {
                ClientFullName = clientFullName,
                ServiceName = string.Join(", ", Cart.Select(x => x.ServiceName)),
                DateReceived = DateTime.Now,
                TotalPrice = Cart.Sum(x => x.TotalPrice),
                Status = "Новый"
            });

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

        public CartItem AddToCart(ServiceCatalogRecord service, int quantity)
        {
            if (service == null)
            {
                throw new InvalidOperationException("Услуга не выбрана.");
            }

            if (quantity <= 0)
            {
                throw new InvalidOperationException("Количество должно быть больше нуля.");
            }

            var existing = Cart.FirstOrDefault(x => x.ServiceId == service.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
                return existing;
            }

            var item = new CartItem
            {
                Id = _nextCartId++,
                ServiceId = service.Id,
                ServiceName = service.Name,
                UnitPrice = service.BasePrice,
                Quantity = quantity
            };
            Cart.Add(item);
            return item;
        }

        public void RemoveFromCart(int cartItemId)
        {
            var item = Cart.FirstOrDefault(x => x.Id == cartItemId);
            if (item != null)
            {
                Cart.Remove(item);
            }
        }

        public void ClearCart()
        {
            Cart.Clear();
        }

        public ServiceCatalogRecord AddService(ServiceCatalogRecord service)
        {
            service.Id = ServicesCatalog.Count == 0 ? 1 : ServicesCatalog.Max(x => x.Id) + 1;
            ServicesCatalog.Add(service);
            return service;
        }

        public void DeleteService(int id)
        {
            var service = ServicesCatalog.FirstOrDefault(x => x.Id == id);
            if (service != null)
            {
                ServicesCatalog.Remove(service);
            }
        }

        public void UpdateService(ServiceCatalogRecord updated)
        {
            var current = ServicesCatalog.FirstOrDefault(x => x.Id == updated.Id);
            if (current == null)
            {
                return;
            }

            current.Name = updated.Name;
            current.Category = updated.Category;
            current.Duration = updated.Duration;
            current.BasePrice = updated.BasePrice;
            current.IsActive = updated.IsActive;
        }

        public ReferenceRecord AddReference(ReferenceRecord reference)
        {
            reference.Id = _nextReferenceId++;
            References.Add(reference);
            return reference;
        }

        public void DeleteReference(int id)
        {
            var reference = References.FirstOrDefault(x => x.Id == id);
            if (reference != null)
            {
                References.Remove(reference);
            }
        }

        public void UpdateReference(ReferenceRecord updated)
        {
            var current = References.FirstOrDefault(x => x.Id == updated.Id);
            if (current == null)
            {
                return;
            }

            current.Type = updated.Type;
            current.Name = updated.Name;
            current.Value = updated.Value;
        }
    }
}
