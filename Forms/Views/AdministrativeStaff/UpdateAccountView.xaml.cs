using Services.DTOs.AccountDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Windows;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for UpdateAccountView.xaml
    /// </summary>
    public partial class UpdateAccountView : Window
    {
        private readonly IAccountService _service;
        private readonly string _username;
        public UpdateAccountView(IAccountService service, string username)
        {
            InitializeComponent();
            _service = service;
            _username = username;
            UsernameInput.Text = username;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegisterAccount_Click(object sender, RoutedEventArgs e)
        {
            UpdateAccountDTO dto = new()
            {
                Password = PasswordInput.Password,
                ComfirmPassword = ConfirmPasswordInput.Password
            };
            _service.UpdateAccount(_username, dto);
            MessageBox.Show("Thay đổi mật khẩu thành công");
            this.Close();
        }
    }
}
