using Forms.Views.AdministrativeStaff;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Forms.Views.AdministrativeStaff
{
    /// <summary>
    /// Interaction logic for AdministrativeStaffLayoutView.xaml
    /// </summary>
    public partial class AdministrativeStaffLayoutView : Window
    {
        public AdministrativeStaffLayoutView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new GeneralInfoView());
        }

        private void RegulationViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new RegulationView());
        }

        private void GeneralInfoViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new GeneralInfoView());
        }

        // Hàm để load UserControl vào RenderPages
        private void LoadUserControl(UserControl userControl)
        {
            // Xóa tất cả các phần tử con hiện tại trong RenderPages
            RenderPages.Children.Clear();

            // Thêm UserControl vào RenderPages
            RenderPages.Children.Add(userControl);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Sidebar_MouseEnter(object sender, MouseEventArgs e)
        {
            // Mở rộng sidebar khi chuột di chuyển vào
            Storyboard storyboard = (Storyboard)this.Resources["ExpandSidebar"];
            storyboard.Begin();

            // Hiển thị các text khi sidebar mở rộng
            RegulationText.Visibility = Visibility.Visible;
            GeneralInfoText.Visibility = Visibility.Visible;
            // Các text khác nếu có
        }

        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
        {
            // Thu nhỏ sidebar khi chuột di chuyển ra ngoài
            Storyboard storyboard = (Storyboard)this.Resources["CollapseSidebar"];
            storyboard.Begin();

            // Ẩn các text khi sidebar thu nhỏ
            RegulationText.Visibility = Visibility.Collapsed;
            GeneralInfoText.Visibility = Visibility.Collapsed;
            // Ẩn các text khác nếu có
        }
    }
}
