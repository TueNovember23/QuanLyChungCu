using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.ViewModels.Accountant;
using Services.DTOs.LoginDTO;
using Services.Interfaces.SharedServices;
using System.Threading.Tasks;
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

        public string PasswordEyeIcon => IsPasswordVisible ? "🙈" : "👁️";

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
                        MainWindow mainWindow = new(response.Username);
                        mainWindow.Show();

                        _loginWindow.Close();
                    }
                    else
                    {
                        MessageBox.Show("Login success");
                    }
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
