using Forms.ViewModels.ServiceSupervisor;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Forms.Views.ServiceSupervisor
{
    /// <summary>
    /// Interaction logic for RegisterParkingView.xaml
    /// </summary>
    public partial class RegisterParkingView : UserControl
    {
        public RegisterParkingView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<RegisterParkingViewModel>();
        }
    }
}