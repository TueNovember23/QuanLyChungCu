using Core;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace Forms
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider? ServiceProvider { get; private set; }

        public App()
        {
            ConfigureServices();
            InitializeComponent();
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is BusinessException businessException)
            {
                MessageBox.Show(businessException.Message, "Lỗi nghiệp vụ", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
            }
            else
            {
                MessageBox.Show($"Đã xảy ra lỗi không xác định. Vui lòng liên hệ bộ phận hỗ trợ: {e.Exception.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            }
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();
            ServiceRegistration.ConfigureServices(services); 
            ServiceProvider = services.BuildServiceProvider(); 
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
