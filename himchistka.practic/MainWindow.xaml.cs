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
                var order = _repository.CreateOrderFromCart(clientName);
                var itemsCopy = _repository.Cart.Select(x => new CartItem
                {
                    Id = x.Id,
                    ServiceId = x.ServiceId,
                    ServiceName = x.ServiceName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                }).ToList();

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
            var baseService = _repository.ServicesCatalog.FirstOrDefault();
            var order = _repository.AddOrder(new OrderRecord
            {
                ClientFullName = _currentUser?.FullName ?? "Новый клиент",
                ServiceName = baseService?.Name ?? "Новая услуга",
                DateReceived = DateTime.Now,
                TotalPrice = baseService?.BasePrice ?? 1000,
                Status = "Новый"
            });

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

            selected.Status = selected.Status == "Новый" ? "В работе" : "Готов";
            selected.TotalPrice += 100;
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

            _authService.DeleteUser(selected.Id);
            _users.Remove(selected);
            SetStatus("Пользователь удален.", false);
        }

        private void AddReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            _repository.AddReference(new ReferenceRecord
            {
                Type = "Категория",
                Name = "Новая справка",
                Value = "Значение"
            });
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

            selected.Value = "Обновлено";
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

            var statusOk = statusFilter == "Все статусы" || item.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase);
            var searchOk = string.IsNullOrWhiteSpace(search)
                           || item.ClientFullName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                           || item.ServiceName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

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

        private void SetStatus(string message, bool isError)
        {
            StatusMessageTextBlock.Text = message;
            StatusMessageTextBlock.Foreground = isError
                ? System.Windows.Media.Brushes.DarkRed
                : System.Windows.Media.Brushes.DarkGreen;
        }
    }
}
