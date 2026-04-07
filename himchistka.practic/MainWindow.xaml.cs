using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace himchistka.practic
{
    public partial class MainWindow : Window
    {
        private readonly AuthService _authService = new AuthService();
        private readonly OrderRepository _repository = new OrderRepository();
        private UserAccount _currentUser;
        private ICollectionView _ordersView;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            ApplyPermissions();
        }

        private void InitializeData()
        {
            OrdersDataGrid.ItemsSource = _repository.Orders;
            ClientsDataGrid.ItemsSource = _repository.Clients;
            _ordersView = CollectionViewSource.GetDefaultView(OrdersDataGrid.ItemsSource);
            _ordersView.SortDescriptions.Add(new SortDescription(nameof(OrderRecord.DateReceived), ListSortDirection.Descending));
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _authService.Login(LoginTextBox.Text, PasswordBox.Password);
                if (user == null)
                {
                    SetStatus("Ошибка: неверный логин или пароль.", true);
                    return;
                }

                _currentUser = user;
                CurrentRoleTextBlock.Text = $"{user.Role} ({user.FullName})";
                ApplyPermissions();
                SetStatus("Вход выполнен успешно.", false);
            }
            catch (Exception ex)
            {
                SetStatus($"Ошибка авторизации: {ex.Message}", true);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var role = ParseRole((RegisterRoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString());
                _authService.Register(RegisterNameTextBox.Text, RegisterLoginTextBox.Text, RegisterPasswordBox.Password, role);
                SetStatus("Регистрация выполнена успешно. Теперь выполните вход.", false);
            }
            catch (Exception ex)
            {
                SetStatus($"Ошибка регистрации: {ex.Message}", true);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshView();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            RefreshView();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplySorting();
            RefreshView();
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CanAdd())
            {
                SetStatus("Недостаточно прав для добавления записи.", true);
                return;
            }

            var newOrder = new OrderRecord
            {
                ClientFullName = "Новый клиент",
                ServiceName = "Новая услуга",
                DateReceived = DateTime.Today,
                TotalPrice = 1000,
                Status = "Новый"
            };

            if (!ValidationService.IsValidOrder(newOrder))
            {
                SetStatus("Некорректные данные нового заказа.", true);
                return;
            }

            _repository.AddOrder(newOrder);
            RefreshView();
            SetStatus("Новая запись добавлена.", false);
        }

        private void EditOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CanEdit())
            {
                SetStatus("Недостаточно прав для редактирования записи.", true);
                return;
            }

            var selected = OrdersDataGrid.SelectedItem as OrderRecord;
            if (selected == null)
            {
                SetStatus("Выберите запись для редактирования.", true);
                return;
            }

            selected.Status = selected.Status == "Готов" ? "В работе" : "Готов";
            selected.TotalPrice += 100;
            RefreshView();
            SetStatus("Запись обновлена (демо-редактирование).", false);
        }

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CanDelete())
            {
                SetStatus("Недостаточно прав для удаления записи.", true);
                return;
            }

            var selected = OrdersDataGrid.SelectedItem as OrderRecord;
            if (selected == null)
            {
                SetStatus("Выберите запись для удаления.", true);
                return;
            }

            _repository.DeleteOrder(selected.Id);
            RefreshView();
            SetStatus("Запись удалена.", false);
        }

        private void RefreshView()
        {
            _ordersView.Filter = x =>
            {
                var item = x as OrderRecord;
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
            };

            _ordersView.Refresh();
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

        private void ApplyPermissions()
        {
            var isAdmin = _currentUser?.Role == UserRole.Admin;
            var isManager = _currentUser?.Role == UserRole.Manager;
            var isUser = _currentUser?.Role == UserRole.User;

            ClientsTab.Visibility = (isAdmin || isManager) ? Visibility.Visible : Visibility.Collapsed;

            AddOrderButton.IsEnabled = isAdmin || isManager;
            EditOrderButton.IsEnabled = isAdmin || isManager;
            DeleteOrderButton.IsEnabled = isAdmin;

            SearchTextBox.IsEnabled = isAdmin || isManager || isUser;
            SortComboBox.IsEnabled = isAdmin || isManager || isUser;
            StatusFilterComboBox.IsEnabled = isAdmin || isManager || isUser;

            if (_currentUser == null)
            {
                CurrentRoleTextBlock.Text = "Не авторизован";
            }
        }

        private bool CanAdd()
        {
            return _currentUser != null && (_currentUser.Role == UserRole.Admin || _currentUser.Role == UserRole.Manager);
        }

        private bool CanEdit()
        {
            return _currentUser != null && (_currentUser.Role == UserRole.Admin || _currentUser.Role == UserRole.Manager);
        }

        private bool CanDelete()
        {
            return _currentUser != null && _currentUser.Role == UserRole.Admin;
        }

        private static UserRole ParseRole(string roleText)
        {
            UserRole role;
            return Enum.TryParse(roleText, out role) ? role : UserRole.User;
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
