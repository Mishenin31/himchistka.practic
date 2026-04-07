using System;
using System.Windows;

namespace himchistka.practic.ApplicationData
{
    public static class AppConnect
    {
        private static CleanersDBEntities _model01;

        public static CleanersDBEntities model01
        {
            get
            {
                if (_model01 == null)
                {
                    try
                    {
                        _model01 = new CleanersDBEntities();
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
            if (CurrentUser == null)
            {
                return false;
            }

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
