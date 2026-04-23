using System.Windows;

namespace himchistka.practic
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OpenAuthButton_Click(object sender, RoutedEventArgs e)
        {
            var authWindow = new AuthWindow
            {
                Owner = this
            };

            Hide();
            authWindow.ShowDialog();
            Show();
        }

        private void OpenRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow
            {
                Owner = this
            };

            Hide();
            registerWindow.ShowDialog();
            Show();
        }
    }
}
