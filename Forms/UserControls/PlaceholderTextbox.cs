using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Forms.UserControls
{
    public static class PlaceholderTextBox
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder", typeof(string), typeof(PlaceholderTextBox),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetPlaceholder(TextBox textBox)
        {
            return (string)textBox.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(TextBox textBox, string value)
        {
            textBox.SetValue(PlaceholderProperty, value);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.GotFocus += RemovePlaceholder;
                textBox.LostFocus += ShowPlaceholder;

                ShowPlaceholder(textBox, new RoutedEventArgs());
            }
        }

        private static void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == GetPlaceholder(textBox))
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black; // Set color back to normal text color
            }
        }

        private static void ShowPlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = GetPlaceholder(textBox);
                textBox.Foreground = Brushes.Gray; // Set placeholder color
            }
        }
    }
}
