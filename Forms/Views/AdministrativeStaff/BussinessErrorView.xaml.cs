using System.Windows;
using System.Windows.Media.Animation;

namespace Forms.Views.AdministrativeStaff
{
    public partial class BusinessErrorView : Window
    {
        public BusinessErrorView(string errorMessage)
        {
            InitializeComponent();
            ErrorMessage.Text = errorMessage;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Hiệu ứng fade-in
            var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(Window.OpacityProperty, fadeInAnimation);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiệu ứng fade-out khi đóng
            var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOutAnimation.Completed += (s, a) => this.Close();
            this.BeginAnimation(Window.OpacityProperty, fadeOutAnimation);
        }
    }
}
