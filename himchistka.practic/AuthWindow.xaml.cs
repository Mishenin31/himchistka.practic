using System;
using System.Windows;

namespace himchistka.practic
{
    public partial class AuthWindow : Window
    {
        private readonly AuthService _authService = new AuthService();

        public AuthWindow()
        {
            InitializeComponent();
            LoginTextBox.Text = "admin";
            PasswordBox.Password = "admin123";
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _authService.Login(LoginTextBox.Text, PasswordBox.Password);
                if (user == null)
                {
                    SetStatus("Неверный логин или пароль.", true);
                    return;
                }

                var mainWindow = new MainWindow(user);
                mainWindow.Show();

                Owner?.Close();
                Close();
            }
            catch (Exception ex)
            {
                SetStatus($"Ошибка авторизации: {ex.Message}", true);
            }
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
