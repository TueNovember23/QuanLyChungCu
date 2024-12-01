using Forms.ViewModels.AdministativeStaff;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for MaintenanceView.xaml
    /// </summary>
    public partial class MaintenanceView : UserControl
    {
        public MaintenanceView(string username)
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<MaintainanceViewModel>()!;
            ((MaintainanceViewModel)DataContext).Username = username;
        }
    }
}
