using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace himchistka.practic
{
    public partial class OrderEditWindow : Window
    {
        public OrderRecord Result { get; private set; }

        public OrderEditWindow(OrderRecord source = null)
        {
            InitializeComponent();

            var order = source ?? new OrderRecord
            {
                DateReceived = DateTime.Now,
                Status = "Новый"
            };

            ClientTextBox.Text = order.ClientFullName;
            ServiceTextBox.Text = order.ServiceName;
            DateReceivedPicker.SelectedDate = order.DateReceived == default(DateTime) ? DateTime.Now : order.DateReceived;
            TotalPriceTextBox.Text = order.TotalPrice.ToString("0.##", CultureInfo.InvariantCulture);
            SelectStatus(order.Status);
        }

        private void SelectStatus(string status)
        {
            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if (string.Equals(item.Content?.ToString(), status, StringComparison.OrdinalIgnoreCase))
                {
                    StatusComboBox.SelectedItem = item;
                    return;
                }
            }

            StatusComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClientTextBox.Text)
                || string.IsNullOrWhiteSpace(ServiceTextBox.Text)
                || !DateReceivedPicker.SelectedDate.HasValue
                || string.IsNullOrWhiteSpace(TotalPriceTextBox.Text))
            {
                MessageBox.Show("Заполните все поля.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal price;
            if (!decimal.TryParse(TotalPriceTextBox.Text.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out price) || price <= 0)
            {
                MessageBox.Show("Введите корректную сумму.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Result = new OrderRecord
            {
                ClientFullName = ClientTextBox.Text.Trim(),
                ServiceName = ServiceTextBox.Text.Trim(),
                DateReceived = DateReceivedPicker.SelectedDate.Value,
                TotalPrice = price,
                Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Новый"
            };

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
