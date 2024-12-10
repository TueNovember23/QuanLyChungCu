using Forms.Views.AdministrativeStaff;
using Forms.Views.ServiceSupervisor;
using Microsoft.Extensions.DependencyInjection;
using Services.DTOs.LoginDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Forms.Views.Accountant
{
    /// <summary>
    /// Interaction logic for AccountantLayoutView.xaml
    /// </summary>
    public partial class AccountantLayoutView : Window
    {
        public LoginResponseDTO? User { get; set; }

        public AccountantLayoutView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new ViolationView());
        }

        private void ViolationViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new ViolationView());
        }

        private void PaymentViewButton_Click(object sender, RoutedEventArgs e)
        {

            LoadUserControl(new PaymentView());
        }

        private void InvoiceViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new InvoiceView());
        }

        private void EquipmentViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new EquipmentView());
        }

        private void RepairInvoiceViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new RepairInvoiceView());
        }

        private void LoadUserControl(UserControl userControl)
        {
            RenderPages.Children.Clear();

            RenderPages.Children.Add(userControl);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            LoginView f = new();
            f.Show();
            this.Close();
        }

        private void Sidebar_MouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard storyboard = (Storyboard)this.Resources["ExpandSidebar"];
            storyboard.Begin();

            ResistorText.Visibility = Visibility.Visible;
            CircleSliceText.Visibility = Visibility.Visible;
            CalendarText.Visibility = Visibility.Visible;
            EqualizerText.Visibility = Visibility.Visible;
            ChatText.Visibility = Visibility.Visible;
            YourAccountText.Visibility = Visibility.Visible;
            ExitText.Visibility = Visibility.Visible;
        }

        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard storyboard = (Storyboard)this.Resources["CollapseSidebar"];
            storyboard.Begin();

            ResistorText.Visibility = Visibility.Collapsed;
            CircleSliceText.Visibility = Visibility.Collapsed;
            CalendarText.Visibility = Visibility.Collapsed;
            EqualizerText.Visibility = Visibility.Collapsed;
            ChatText.Visibility = Visibility.Collapsed;
            YourAccountText.Visibility = Visibility.Collapsed;
            ExitText.Visibility = Visibility.Collapsed;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            IAccountService accountService = App.ServiceProvider?.GetService<IAccountService>()!;
            UpdateAccountView mv = new(accountService, User!.Username);
            mv.ShowDialog();
        }
    }
}
