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
        private bool isMaximized = false;
        private const double SIDEBAR_COLLAPSED_WIDTH = 60;
        private const double SIDEBAR_EXPANDED_WIDTH = 200;

        public AdministrativeStaffLayoutView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sidebar.Width = SIDEBAR_COLLAPSED_WIDTH;
            HideSidebarTexts();
            LoadUserControl(new ApartmentView());
        }

        private void ApartmentViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new ApartmentView());
        }

        private void DepartmentViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new DepartmentView());
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
            RenderPages.Children.Clear();

            RenderPages.Children.Add(userControl);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Sidebar_MouseEnter(object sender, MouseEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("ExpandSidebar");
            storyboard.Begin();
            ShowSidebarTexts();
        }

        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("CollapseSidebar");
            storyboard.Begin();
            HideSidebarTexts();
        }


        private void Window_StateChanged(object sender, EventArgs e)
        {
            isMaximized = (WindowState == WindowState.Maximized);
            // Keep sidebar collapsed when window state changes
            Sidebar.Width = SIDEBAR_COLLAPSED_WIDTH;
            HideSidebarTexts();
        }

        private void ShowSidebarTexts()
        {
            ApartmentText.Visibility = Visibility.Visible;
            RegulationText.Visibility = Visibility.Visible;
            GeneralInfoText.Visibility = Visibility.Visible;
            CalendarText.Visibility = Visibility.Visible;
            EqualizerText.Visibility = Visibility.Visible;
            ChatText.Visibility = Visibility.Visible;
            ExitText.Visibility = Visibility.Visible;
        }

        private void HideSidebarTexts()
        {
            ApartmentText.Visibility = Visibility.Collapsed;
            RegulationText.Visibility = Visibility.Collapsed;
            GeneralInfoText.Visibility = Visibility.Collapsed;
            CalendarText.Visibility = Visibility.Collapsed;
            EqualizerText.Visibility = Visibility.Collapsed;
            ChatText.Visibility = Visibility.Collapsed;
            ExitText.Visibility = Visibility.Collapsed;
        }


    }
}
