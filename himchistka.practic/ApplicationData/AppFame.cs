using System.Windows.Controls;

namespace Cleaners.ApplicationData
{
    /// <summary>
    /// Статический класс для управления навигацией между страницами
    /// </summary>
    public static class AppFrame
    {
        /// <summary>
        /// Статическая ссылка на главный фрейм приложения
        /// </summary>
        public static Frame frmMain;

        /// <summary>
        /// Навигация к странице
        /// </summary>
        public static void Navigate(Page page)
        {
            if (frmMain != null)
            {
                frmMain.Navigate(page);
            }
        }

        /// <summary>
        /// Навигация к странице по типу
        /// </summary>
        public static void Navigate<T>() where T : Page, new()
        {
            Navigate(new T());
        }

        /// <summary>
        /// Возврат на предыдущую страницу
        /// </summary>
        public static void GoBack()
        {
            if (frmMain != null && frmMain.CanGoBack)
            {
                frmMain.GoBack();
            }
        }
    }
}