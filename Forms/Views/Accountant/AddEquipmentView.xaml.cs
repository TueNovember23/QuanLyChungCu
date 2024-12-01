using Services.Interfaces.AccountantServices;
using System.Windows;
using Forms.ViewModels.Accountant;

namespace Forms.Views.Accountant
{
    /// <summary>
    /// Interaction logic for AddEquipmentView.xaml
    /// </summary>
    public partial class AddEquipmentView : Window
    {
        public AddEquipmentView(IEquipmentService equipmentService, string equipmentCode)
        {
            InitializeComponent();
            DataContext = new AddEquipmentViewModel(equipmentService, equipmentCode);
        }
    }
}
