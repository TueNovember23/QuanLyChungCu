using System.Windows;

namespace Forms.ViewModels.Accountant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Username { get; private set; }
        public MainWindow(string username)
        {
            InitializeComponent();
            Username = username;
        }
    }
}
