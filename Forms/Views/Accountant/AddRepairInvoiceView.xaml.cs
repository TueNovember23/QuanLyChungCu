using Forms.ViewModels.Accountant;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]*(\.[0-9]*)?$"); 
            e.Handled = !regex.IsMatch((sender as TextBox)?.Text.Insert((sender as TextBox)?.CaretIndex ?? 0, e.Text));
        }

        private void NumberOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; 
            }
        }
    }
}
