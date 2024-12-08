using Forms.ViewModels.AdministativeStaff;
using Microsoft.Extensions.DependencyInjection;
using Services.DTOs.AccountDTO;
using Services.DTOs.LoginDTO;
using System.Windows.Controls;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for MaintenanceView.xaml
    /// </summary>
    public partial class MaintenanceView : UserControl
    {
        private LoginResponseDTO? _user;
        public LoginResponseDTO? User {
            get { return _user; } 
            set
            {
                _user = value;
                (DataContext as MaintainanceViewModel)!.User = User;
            }
        }

        public MaintenanceView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<MaintainanceViewModel>()!;
        }
    }
}
