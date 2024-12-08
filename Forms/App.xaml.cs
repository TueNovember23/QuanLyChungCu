﻿using Core;
using Forms.Views.AdministrativeStaff;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces.ServiceSupervisorServices;
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
            TaskScheduler.UnobservedTaskException += (sender, exceptionEvent) =>
            {
                if (exceptionEvent.Exception.InnerException is BusinessException businessException)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Hiển thị lỗi bằng BusinessErrorView
                        BusinessErrorView errorView = new BusinessErrorView(businessException.Message);
                        errorView.ShowDialog();
                    });
                    exceptionEvent.SetObserved(); // Đánh dấu ngoại lệ đã được xử lý
                }
            };
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is BusinessException businessException)
            {
                BusinessErrorView f = new(businessException.Message);
                f.ShowDialog();
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
