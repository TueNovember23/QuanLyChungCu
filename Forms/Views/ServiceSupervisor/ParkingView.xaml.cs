using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Forms.Views.ServiceSupervisor
{
    public partial class ParkingView : UserControl
    {
        public ParkingView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RenderHeader.Children.Clear();
            RenderHeader.Children.Add(new Header());
        }

        private void Header_Loaded()
        {

        }
    }
}