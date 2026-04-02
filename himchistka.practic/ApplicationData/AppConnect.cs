using System;
using System.Linq;
using System.Windows;

namespace Cleaners.ApplicationData
{
    public static class AppConnect
    {
        private static PasswordHelper _model01;

        public static PasswordHelper model01
        {
            get
            {
                if (_model01 == null)
                {
                    try
                    {
                        _model01 = new PasswordHelper();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось подключиться к базе данных:\n{ex.Message}",
                            "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                return _model01;
            }
        }

        public static Employees CurrentUser { get; set; }

        public static bool IsConnectionAvailable()
        {
            try
            {
                return model01 != null && model01.Database.Exists();
            }
            catch
            {
                return false;
            }
        }

        public static bool IsUserInRole(string role)
        {
            if (CurrentUser == null) return false;
            return CurrentUser.Role == role;
        }

        public static bool IsAdmin => CurrentUser?.Role == "Admin";
        public static bool IsManager => CurrentUser?.Role == "Manager" || CurrentUser?.Role == "Admin";

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}