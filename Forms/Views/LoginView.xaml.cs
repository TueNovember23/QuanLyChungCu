using Forms.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Forms.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider?.GetService<LoginViewModel>();
            viewModel?.Initialize(this);
            DataContext = viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                if (sender is PasswordBox passwordBox)
                {
                    viewModel.Password = passwordBox.Password;
                }
            }
        }
        

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowPasswordButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.IsPasswordVisible = true;
            }
        }

        private void ShowPasswordButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.IsPasswordVisible = false; 
            }
        }
    }
}
