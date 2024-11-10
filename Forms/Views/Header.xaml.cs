using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Forms.Views
{
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }

        // Sự kiện MouseDown để di chuyển cửa sổ
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.DragMove();
                }
            }
        }

    }
}
