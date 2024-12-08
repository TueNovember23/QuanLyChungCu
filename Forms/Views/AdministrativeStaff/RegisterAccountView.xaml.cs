using Services.DTOs.AccountDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Windows;
using System.Windows.Controls;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for RegisterAccountView.xaml
    /// </summary>
    public partial class RegisterAccountView : Window
    {
        private readonly IAccountService _service;
        public RegisterAccountView(IAccountService service)
        {
            InitializeComponent();
            _service = service;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            RoleInput.ItemsSource = (await _service.GetAllRole()).Select(_ => _.RoleName);
        }

        private void RegisterAccount_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountDTO dto = new()
            {
                Username = UsernameInput.Text,
                Password = PasswordInput.Password,
                FullName = FullNameInput.Text,
                Role = RoleInput.SelectedItem.ToString()
            };
            _ = _service.Create(dto);
            MessageBox.Show("Tạo tài khoản thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
