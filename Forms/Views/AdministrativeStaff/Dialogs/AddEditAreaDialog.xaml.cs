using Forms.ViewModels.AdministativeStaff.GeneralInfo;
using Forms.ViewModels.ServiceSupervisor;
using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Forms.Views.AdministrativeStaff.Dialogs
{
    /// <summary>
    /// Interaction logic for AddEditAreaDialog.xaml
    /// </summary>
    public partial class AddEditAreaDialog : UserControl
    {
        public AddEditAreaDialog()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider?.GetService<AddEditAreaViewModel>();
        }
    }
}
