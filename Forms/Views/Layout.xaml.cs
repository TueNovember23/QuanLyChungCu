using Forms.Views.ServiceSupervisor;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Forms.Views.AdministrativeStaff;

namespace Forms.Views
{
    public partial class Layout : Window
    {
        public Layout()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RenderPages.Children.Clear();
            RenderPages.Children.Add(new GeneralInfoView());

            //RenderPages.Children.Add(new RegisterParkingView());
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Sidebar_MouseEnter(object sender, MouseEventArgs e)
        {
            // Expand sidebar when mouse enters
            Storyboard storyboard = (Storyboard)this.Resources["ExpandSidebar"];
            storyboard.Begin();

            // Show text labels when sidebar expands
            ResistorText.Visibility = Visibility.Visible;
            CircleSliceText.Visibility = Visibility.Visible;
            CalendarText.Visibility = Visibility.Visible;
            EqualizerText.Visibility = Visibility.Visible;
            ChatText.Visibility = Visibility.Visible;
            ExitText.Visibility = Visibility.Visible;
        }

        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
        {
            // Collapse sidebar when mouse leaves
            Storyboard storyboard = (Storyboard)this.Resources["CollapseSidebar"];
            storyboard.Begin();

            // Hide text labels when sidebar collapses
            ResistorText.Visibility = Visibility.Collapsed;
            CircleSliceText.Visibility = Visibility.Collapsed;
            CalendarText.Visibility = Visibility.Collapsed;
            EqualizerText.Visibility = Visibility.Collapsed;
            ChatText.Visibility = Visibility.Collapsed;
            ExitText.Visibility = Visibility.Collapsed;
        }
    }
}
