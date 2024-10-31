using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.DTOs.LoginDTO;
using Services.Interfaces.SharedServices;
using System.Windows;

namespace Forms.ViewModels
{
    public partial class LoginViewModel(ILoginService loginService) : ObservableObject
    {
        private readonly ILoginService _loginService = loginService;

        [ObservableProperty]
        private string username = "";

        [ObservableProperty]
        private string password = "";

        [ObservableProperty]
        private bool isBusy;

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
                    MessageBox.Show("Login success");
                }
                else
                {
                    MessageBox.Show("Login failed");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
