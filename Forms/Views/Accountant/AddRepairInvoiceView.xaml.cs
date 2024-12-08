using Forms.ViewModels.Accountant;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Forms.Views.Accountant
{
    /// <summary>
    /// Interaction logic for AddRepairInvoiceView.xaml
    /// </summary>
    public partial class AddRepairInvoiceView : Window
    {

        public AddRepairInvoiceView(IRepairInvoiceService repairInvoiceService)
        {
            InitializeComponent();

            DataContext = new AddRepairInvoiceViewModel(repairInvoiceService);
        }


        private void SearchMalfunctionEquipmentInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is AddRepairInvoiceViewModel viewModel)
            {
                viewModel.OnSearchTextChanged(); 
            }
        }

        private void SaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddRepairInvoiceViewModel viewModel)
            {
                viewModel.SaveInvoiceCommand.Execute(null); 
            }
        }
    }
}
