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
        private readonly OrderRepository _repository = new OrderRepository();
        private readonly UserAccount _currentUser;
        private ICollectionView _ordersView;

        public MainWindow()
            : this(null)
        {
        }

        public MainWindow(UserAccount currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            InitializeData();
            FillProfileData();
            ApplyPermissions();
            SetStatus($"Авторизация выполнена: {_currentUser?.FullName ?? "гость"}.", false);
        }

        private void InitializeData()
        {
            OrdersDataGrid.ItemsSource = _repository.Orders;
            ClientsDataGrid.ItemsSource = _repository.Clients;
            _ordersView = CollectionViewSource.GetDefaultView(OrdersDataGrid.ItemsSource);
            _ordersView.SortDescriptions.Add(new SortDescription(nameof(OrderRecord.DateReceived), ListSortDirection.Descending));
            UpdateDashboard();
            UpdateStatistics();
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
            UpdateDashboard();
            UpdateStatistics();
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
            UpdateDashboard();
            UpdateStatistics();
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
            UpdateDashboard();
            UpdateStatistics();
            SetStatus("Запись удалена.", false);
        }

        private void PreviousTabButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateTab(-1);
        }

        private void NextTabButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateTab(1);
        }

        private void TablesTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTabNavigationButtons();
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                SetStatus("Гостевой режим: редактирование профиля недоступно.", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(ProfileFullNameTextBox.Text))
            {
                SetStatus("Введите ФИО сотрудника.", true);
                return;
            }

            var newPassword = ProfilePasswordBox.Password?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(newPassword) && !ValidationService.IsValidPassword(newPassword))
            {
                SetStatus("Пароль должен содержать минимум 6 символов.", true);
                return;
            }

            _currentUser.FullName = ProfileFullNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(newPassword))
            {
                _currentUser.PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPassword)));
            }

            CurrentRoleTextBlock.Text = $"{_currentUser.Role} ({_currentUser.FullName})";
            ProfilePasswordBox.Clear();
            ProfileLastUpdateTextBlock.Text = $"Данные обновлены: {DateTime.Now:dd.MM.yyyy HH:mm}";
            UpdateProfileSummary();
            SetStatus("Данные личного кабинета сохранены.", false);
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
            UpdateDashboard();
            UpdateStatistics();
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
            StatisticsTab.Visibility = (isAdmin || isManager) ? Visibility.Visible : Visibility.Collapsed;
            ProfileTab.Visibility = _currentUser != null ? Visibility.Visible : Visibility.Collapsed;

            AddOrderButton.IsEnabled = isAdmin || isManager;
            EditOrderButton.IsEnabled = isAdmin || isManager;
            DeleteOrderButton.IsEnabled = isAdmin;

            SearchTextBox.IsEnabled = isAdmin || isManager || isUser;
            SortComboBox.IsEnabled = isAdmin || isManager || isUser;
            StatusFilterComboBox.IsEnabled = isAdmin || isManager || isUser;

            CurrentRoleTextBlock.Text = _currentUser == null
                ? "Не авторизован"
                : $"{_currentUser.Role} ({_currentUser.FullName})";

            UpdateTabNavigationButtons();
        }

        private void FillProfileData()
        {
            ProfileFullNameTextBox.Text = _currentUser?.FullName ?? "Гость";
            ProfileLoginTextBox.Text = _currentUser?.Login ?? "Нет логина";
            ProfileRoleTextBox.Text = _currentUser?.Role.ToString() ?? "Guest";
            ProfileLastUpdateTextBlock.Text = "Изменений пока нет.";
            UpdateProfileSummary();
        }

        private void UpdateProfileSummary()
        {
            ProfileSummaryTextBlock.Text =
                $"Сотрудник: {ProfileFullNameTextBox.Text}\n" +
                $"Логин: {ProfileLoginTextBox.Text}\n" +
                $"Роль в системе: {ProfileRoleTextBox.Text}";
        }

        private void UpdateDashboard()
        {
            var filteredOrders = _ordersView?.Cast<OrderRecord>().ToList() ?? _repository.Orders.ToList();
            ActiveOrdersTextBlock.Text = filteredOrders.Count.ToString();
            var average = filteredOrders.Count == 0 ? 0 : filteredOrders.Average(x => x.TotalPrice);
            AverageCheckTextBlock.Text = $"{average:N0} ₽";
        }

        private void UpdateStatistics()
        {
            NewOrdersCountTextBlock.Text = $"Новые: {_repository.Orders.Count(x => x.Status == "Новый")}";
            InProgressOrdersCountTextBlock.Text = $"В работе: {_repository.Orders.Count(x => x.Status == "В работе")}";
            ReadyOrdersCountTextBlock.Text = $"Готово: {_repository.Orders.Count(x => x.Status == "Готов")}";
            DeliveredOrdersCountTextBlock.Text = $"Выдано: {_repository.Orders.Count(x => x.Status == "Выдан")}";
            ClientsCountTextBlock.Text = $"Всего клиентов: {_repository.Clients.Count}";

            var topClient = _repository.Orders
                .GroupBy(x => x.ClientFullName)
                .OrderByDescending(x => x.Sum(o => o.TotalPrice))
                .FirstOrDefault();

            TopClientTextBlock.Text = topClient == null
                ? "Нет данных по клиентам."
                : $"Топ-клиент по выручке: {topClient.Key} ({topClient.Sum(x => x.TotalPrice):N0} ₽)";
        }

        private void NavigateTab(int direction)
        {
            if (TablesTabControl == null)
            {
                return;
            }

            var visibleTabs = TablesTabControl.Items
                .OfType<TabItem>()
                .Where(tab => tab.Visibility == Visibility.Visible)
                .ToList();

            if (visibleTabs.Count <= 1)
            {
                return;
            }

            var selectedTab = TablesTabControl.SelectedItem as TabItem;
            var currentIndex = visibleTabs.IndexOf(selectedTab);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var targetIndex = (currentIndex + direction + visibleTabs.Count) % visibleTabs.Count;
            TablesTabControl.SelectedItem = visibleTabs[targetIndex];
            UpdateTabNavigationButtons();
        }

        private void UpdateTabNavigationButtons()
        {
            if (TablesTabControl == null)
            {
                return;
            }

            var visibleTabsCount = TablesTabControl.Items
                .OfType<TabItem>()
                .Count(tab => tab.Visibility == Visibility.Visible);

            var canNavigate = visibleTabsCount > 1;
            PreviousTabButton.IsEnabled = canNavigate;
            NextTabButton.IsEnabled = canNavigate;
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

        private void SetStatus(string message, bool isError)
        {
            StatusMessageTextBlock.Text = message;
            StatusMessageTextBlock.Foreground = isError
                ? System.Windows.Media.Brushes.DarkRed
                : System.Windows.Media.Brushes.DarkGreen;
        }
    }
}
