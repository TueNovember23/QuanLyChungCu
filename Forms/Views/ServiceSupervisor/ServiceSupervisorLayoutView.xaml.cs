using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Forms.Views.ServiceSupervisor
{
    public partial class ServiceSupervisorLayoutView : Window
    {
        public ServiceSupervisorLayoutView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new ParkingView());
        }

        private void ParkingViewButton_Click(object sender, RoutedEventArgs e)
        {
            
            LoadUserControl(new ParkingView());
        }

        private void RegisterParkingViewButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new RegisterParkingView());
        }

        private void EquipmentViewButton_Click(object sender, RoutedEventArgs e)
        {

            LoadUserControl(new EquipmentView());
        }

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
            Storyboard storyboard = (Storyboard)this.Resources["ExpandSidebar"];
            storyboard.Begin();

            ResistorText.Visibility = Visibility.Visible;
            CircleSliceText.Visibility = Visibility.Visible;
            CalendarText.Visibility = Visibility.Visible;
            EqualizerText.Visibility = Visibility.Visible;
            ChatText.Visibility = Visibility.Visible;
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
            ExitText.Visibility = Visibility.Collapsed;
        }
    }
}
