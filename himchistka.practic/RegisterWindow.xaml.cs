using System;
using System.Windows;
using System.Windows.Controls;

namespace himchistka.practic
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService = new AuthService();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var role = ParseRole((RegisterRoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString());
                _authService.Register(RegisterNameTextBox.Text, RegisterLoginTextBox.Text, RegisterPasswordBox.Password, role);
                SetStatus("Пользователь зарегистрирован. Теперь выполните вход.", false);
            }
            catch (Exception ex)
            {
                SetStatus($"Ошибка регистрации: {ex.Message}", true);
            }
        }

        private static UserRole ParseRole(string roleText)
        {
            UserRole role;
            return Enum.TryParse(roleText, out role) ? role : UserRole.User;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetStatus(string message, bool isError)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Foreground = isError
                ? System.Windows.Media.Brushes.DarkRed
                : System.Windows.Media.Brushes.DarkGreen;
        }
    }
}
