using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Forms.ViewModels
{
    public class CommonViewModel : ObservableObject
    {
        public ICommand MinimizeCommand { get; }
        //public ICommand MaximizeRestoreCommand { get; }
        public ICommand CloseCommand { get; }

        public CommonViewModel()
        {
            MinimizeCommand = new RelayCommand<Window?>(Minimize);
            //MaximizeRestoreCommand = new RelayCommand<Window?>(MaximizeRestore);
            CloseCommand = new RelayCommand<Window?>(Close);
        }

        private void Minimize(Window? window)
        {
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        //private void MaximizeRestore(Window? window)
        //{
        //    if (window != null)
        //    {
        //        window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        //    }
        //}

        private void Close(Window? window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
