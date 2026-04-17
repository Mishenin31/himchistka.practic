using System.Windows;

namespace himchistka.practic
{
    public partial class ReferenceEditWindow : Window
    {
        public ReferenceRecord Result { get; private set; }

        public ReferenceEditWindow(ReferenceRecord source = null)
        {
            InitializeComponent();

            TypeTextBox.Text = source?.Type;
            NameTextBox.Text = source?.Name;
            ValueTextBox.Text = source?.Value;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TypeTextBox.Text)
                || string.IsNullOrWhiteSpace(NameTextBox.Text)
                || string.IsNullOrWhiteSpace(ValueTextBox.Text))
            {
                MessageBox.Show("Заполните все поля.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Result = new ReferenceRecord
            {
                Type = TypeTextBox.Text.Trim(),
                Name = NameTextBox.Text.Trim(),
                Value = ValueTextBox.Text.Trim()
            };

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
