using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace himchistka.practic
{
    public partial class MainWindow : Window
    {
        private readonly OrderRepository _repository = new OrderRepository();
        private readonly AuthService _authService = new AuthService();
        private readonly PdfReceiptService _receiptService = new PdfReceiptService();
        private readonly UserAccount _currentUser;

        private ICollectionView _ordersView;
        private ObservableCollection<UserAccount> _users;

        public MainWindow()
            : this(null)
        {
        }

        public MainWindow(UserAccount currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            InitializeData();
            ApplyPermissions();
            TablesTabControl.SelectedItem = ServicesCatalogTab;
            SetStatus($"Авторизация: {_currentUser?.FullName ?? "гость"}", false);
        }

        private void InitializeData()
        {
            _ordersView = new ListCollectionView(_repository.Orders);
            OrdersDataGrid.ItemsSource = _ordersView;
            ServicesCatalogDataGrid.ItemsSource = _repository.ServicesCatalog;
            CartDataGrid.ItemsSource = _repository.Cart;
            ReferencesDataGrid.ItemsSource = _repository.References;
            _users = new ObservableCollection<UserAccount>(_authService.GetUsers());
            UsersDataGrid.ItemsSource = _users;

            ApplySorting();
            RefreshView();
        }

        private void ApplyPermissions()
        {
            var isAdmin = _currentUser?.Role == UserRole.Admin;
            var isUser = _currentUser?.Role == UserRole.User || _currentUser?.Role == UserRole.Manager;

            UsersTab.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            ReferencesTab.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            AddOrderButton.IsEnabled = isAdmin || isUser;
            EditOrderButton.IsEnabled = isAdmin;
            DeleteOrderButton.IsEnabled = isAdmin;

            AddToCartButton.IsEnabled = isAdmin || isUser;
            CreateOrderFromCartButton.IsEnabled = isAdmin || isUser;
            RemoveFromCartButton.IsEnabled = isAdmin || isUser;
            ClearCartButton.IsEnabled = isAdmin || isUser;

            CurrentRoleTextBlock.Text = _currentUser == null
                ? "Не авторизован"
                : $"Роль: {_currentUser.Role} ({_currentUser.FullName})";
        }


        private void UsersSectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersTab.Visibility != Visibility.Visible)
            {
                SetStatus("Раздел пользователей доступен только администратору.", true);
                return;
            }

            TablesTabControl.SelectedItem = UsersTab;
        }

        private void OrdersSectionButton_Click(object sender, RoutedEventArgs e)
        {
            TablesTabControl.SelectedItem = OrdersTab;
        }

        private void ProductsSectionButton_Click(object sender, RoutedEventArgs e)
        {
            TablesTabControl.SelectedItem = ServicesCatalogTab;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshView();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                RefreshView();
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                ApplySorting();
                RefreshView();
            }
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var service = ServicesCatalogDataGrid.SelectedItem as ServiceCatalogRecord;
            if (service == null)
            {
                SetStatus("Выберите услугу в каталоге.", true);
                return;
            }

            _repository.AddToCart(service, 1);
            CartDataGrid.Items.Refresh();
            SetStatus($"Услуга '{service.Name}' добавлена в корзину.", false);
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            var item = CartDataGrid.SelectedItem as CartItem;
            if (item == null)
            {
                SetStatus("Выберите позицию корзины.", true);
                return;
            }

            _repository.RemoveFromCart(item.Id);
            SetStatus("Позиция удалена из корзины.", false);
        }

        private void ClearCartButton_Click(object sender, RoutedEventArgs e)
        {
            _repository.ClearCart();
            SetStatus("Корзина очищена.", false);
        }

        private void CreateOrderFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var clientName = _currentUser?.FullName ?? "Клиент";
                var itemsCopy = _repository.Cart.Select(x => new CartItem
                {
                    Id = x.Id,
                    ServiceId = x.ServiceId,
                    ServiceName = x.ServiceName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                }).ToList();

                var order = _repository.CreateOrderFromCart(clientName);
                var receiptPath = _receiptService.CreateReceipt(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "receipts"), order, itemsCopy);

                _repository.ClearCart();
                RefreshView();
                SetStatus($"Заказ #{order.Id} создан. PDF чек: {receiptPath}", false);
            }
            catch (Exception ex)
            {
                SetStatus($"Ошибка оформления: {ex.Message}", true);
            }
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new OrderEditWindow(new OrderRecord
            {
                ClientFullName = _currentUser?.FullName ?? string.Empty,
                DateReceived = DateTime.Now,
                Status = "Новый"
            })
            {
                Owner = this
            };

            if (window.ShowDialog() != true || window.Result == null)
            {
                return;
            }

            var order = _repository.AddOrder(window.Result);
            RefreshView();
            SetStatus($"Заказ #{order.Id} добавлен.", false);
        }

        private void EditOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersDataGrid.SelectedItem as OrderRecord;
            if (selected == null)
            {
                SetStatus("Выберите заказ.", true);
                return;
            }

            var draft = new OrderRecord
            {
                Id = selected.Id,
                ClientFullName = selected.ClientFullName,
                ServiceName = selected.ServiceName,
                DateReceived = selected.DateReceived,
                TotalPrice = selected.TotalPrice,
                Status = selected.Status
            };

            var window = new OrderEditWindow(draft)
            {
                Owner = this
            };

            if (window.ShowDialog() != true || window.Result == null)
            {
                return;
            }

            selected.ClientFullName = window.Result.ClientFullName;
            selected.ServiceName = window.Result.ServiceName;
            selected.DateReceived = window.Result.DateReceived;
            selected.TotalPrice = window.Result.TotalPrice;
            selected.Status = window.Result.Status;
            RefreshView();
            SetStatus("Заказ изменен.", false);
        }

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersDataGrid.SelectedItem as OrderRecord;
            if (selected == null)
            {
                SetStatus("Выберите заказ.", true);
                return;
            }

            _repository.DeleteOrder(selected.Id);
            RefreshView();
            SetStatus("Заказ удален.", false);
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var login = "user" + DateTime.Now.Ticks.ToString().Substring(10);
                var created = _authService.Register("Новый сотрудник", login, "password123", UserRole.User);
                _users.Add(created);
                SetStatus($"Пользователь {created.Login} создан.", false);
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
        }

        private void MakeUserRoleButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = UsersDataGrid.SelectedItem as UserAccount;
            if (selected == null)
            {
                SetStatus("Выберите пользователя.", true);
                return;
            }

            selected.Role = UserRole.User;
            _authService.UpdateUser(selected);
            UsersDataGrid.Items.Refresh();
            SetStatus("Роль изменена на User.", false);
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = UsersDataGrid.SelectedItem as UserAccount;
            if (selected == null)
            {
                SetStatus("Выберите пользователя.", true);
                return;
            }

            if (string.Equals(selected.Login, "admin", StringComparison.OrdinalIgnoreCase))
            {
                SetStatus("Нельзя удалить встроенного администратора.", true);
                return;
            }

            _authService.DeleteUser(selected.Id);
            _users.Remove(selected);
            SetStatus("Пользователь удален.", false);
        }

        private void AddReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ReferenceEditWindow(new ReferenceRecord
            {
                Type = "Категория",
                Name = string.Empty,
                Value = string.Empty
            })
            {
                Owner = this
            };

            if (window.ShowDialog() != true || window.Result == null)
            {
                return;
            }

            _repository.AddReference(window.Result);
            ReferencesDataGrid.Items.Refresh();
            SetStatus("Запись справочника добавлена.", false);
        }

        private void EditReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ReferencesDataGrid.SelectedItem as ReferenceRecord;
            if (selected == null)
            {
                SetStatus("Выберите запись справочника.", true);
                return;
            }

            var draft = new ReferenceRecord
            {
                Id = selected.Id,
                Type = selected.Type,
                Name = selected.Name,
                Value = selected.Value
            };

            var window = new ReferenceEditWindow(draft)
            {
                Owner = this
            };

            if (window.ShowDialog() != true || window.Result == null)
            {
                return;
            }

            selected.Type = window.Result.Type;
            selected.Name = window.Result.Name;
            selected.Value = window.Result.Value;
            _repository.UpdateReference(selected);
            ReferencesDataGrid.Items.Refresh();
            SetStatus("Справочник обновлен.", false);
        }

        private void DeleteReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ReferencesDataGrid.SelectedItem as ReferenceRecord;
            if (selected == null)
            {
                SetStatus("Выберите запись справочника.", true);
                return;
            }

            _repository.DeleteReference(selected.Id);
            SetStatus("Справочник удален.", false);
        }

        private void TablesTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActiveOrdersTextBlock.Text = _repository.Orders.Count.ToString();
        }

        private void RefreshView()
        {
            _ordersView.Filter = x => MatchesFilters(x as OrderRecord);
            _ordersView.Refresh();
            ActiveOrdersTextBlock.Text = (_ordersView.Cast<OrderRecord>().Count()).ToString();
        }

        private bool MatchesFilters(OrderRecord item)
        {
            if (item == null)
            {
                return false;
            }

            var statusFilter = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Все статусы";
            var search = SearchTextBox.Text?.Trim() ?? string.Empty;

            var status = item.Status ?? string.Empty;
            var clientName = item.ClientFullName ?? string.Empty;
            var serviceName = item.ServiceName ?? string.Empty;

            var statusOk = statusFilter == "Все статусы" || status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase);
            var searchOk = string.IsNullOrWhiteSpace(search)
                           || clientName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                           || serviceName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

            return statusOk && searchOk;
        }

        private void ApplySorting()
        {
            if (_ordersView == null)
            {
                return;
            }

            _ordersView.SortDescriptions.Clear();
            var selected = (SortComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

            if (selected.Contains("возрастание"))
            {
                _ordersView.SortDescriptions.Add(new SortDescription(nameof(OrderRecord.TotalPrice), ListSortDirection.Ascending));
            }
            else if (selected.Contains("убывание"))
            {
                _ordersView.SortDescriptions.Add(new SortDescription(nameof(OrderRecord.TotalPrice), ListSortDirection.Descending));
            }
            else if (selected.Contains("клиенту"))
            {
                _ordersView.SortDescriptions.Add(new SortDescription(nameof(OrderRecord.ClientFullName), ListSortDirection.Ascending));
            }
            else
            {
                _ordersView.SortDescriptions.Add(new SortDescription(nameof(OrderRecord.DateReceived), ListSortDirection.Descending));
            }
        }


        private static string GetNextStatus(string currentStatus)
        {
            if (string.Equals(currentStatus, "Новый", StringComparison.OrdinalIgnoreCase))
            {
                return "В работе";
            }

            if (string.Equals(currentStatus, "В работе", StringComparison.OrdinalIgnoreCase))
            {
                return "Готов";
            }

            if (string.Equals(currentStatus, "Готов", StringComparison.OrdinalIgnoreCase))
            {
                return "Выдан";
            }

            return "Новый";
        }

        private void SetStatus(string message, bool isError)
        {
            StatusMessageTextBlock.Text = message;
            StatusMessageTextBlock.Foreground = isError
                ? System.Windows.Media.Brushes.DarkRed
                : System.Windows.Media.Brushes.DarkGreen;
        }
    }
}
