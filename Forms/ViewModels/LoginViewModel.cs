using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.Accountant;
using Forms.Views.AdministrativeStaff;
using Forms.Views.ServiceSupervisor;
using Services.DTOs.LoginDTO;
using Services.Interfaces.SharedServices;
using System.Windows;

namespace Forms.ViewModels
{
    public partial class LoginViewModel(ILoginService loginService) : ObservableObject
    {
        private readonly ILoginService _loginService = loginService;
        private Window? _loginWindow = null;

        public void Initialize(Window loginWindow)
        {
            _loginWindow = loginWindow;
        }

        [ObservableProperty]
        private string username = "";

        [ObservableProperty]
        private string password = "";

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isPasswordVisible;

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                LoginRequestDTO request = new()
                {
                    Username = Username,
                    Password = Password
                };
                LoginResponseDTO response = await _loginService.Login(request);
                if (response != null)
                {
                    if (response.Role == "Administrator")
                    {
                        AdministrativeStaffLayoutView administrativeStaffLayoutView = new();
                        administrativeStaffLayoutView.User = response;
                        administrativeStaffLayoutView.Show();
                        _loginWindow?.Close();
                    }
                    else if(response.Role == "Accountant")
                    {
                        AccountantLayoutView f = new();
                        f.User = response;
                        f.Show();
                        _loginWindow?.Close();
                    }
                    else if(response.Role == "ServiceSupervisor")
                    {
                        ServiceSupervisorLayoutView f = new();
                        f.User = response;
                        f.Show();
                        _loginWindow?.Close();
                    }
                }
                else
                {
                    throw new Core.BusinessException("Tên đăng nhập hoặc mật khẩu không chính xác");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
