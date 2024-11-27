using Forms.ViewModels.ServiceSupervisor;
using Microsoft.Extensions.DependencyInjection;
using Services.DTOs.CommunityRoomBookingDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System.Windows;
using System.Windows.Controls;


namespace Forms.Views.ServiceSupervisor
{
    public partial class RegisterCommunityRoomView : UserControl
    {
        public RegisterCommunityRoomView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<RegisterCommunityRoomViewModel>()!;
        }
    }
}