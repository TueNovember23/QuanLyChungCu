using Repositories.Repositories.Entities;
using Services.Interfaces.AccountantServices;
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
using System.Windows.Shapes;

namespace Forms.Views.Accountant
{
    /// <summary>
    /// Interaction logic for DetailVehicleInvoice.xaml
    /// </summary>
    public partial class DetailVehicleInvoice : Window
    {
         private IInvoiceService _invoiceService;
        private VechicleInvoice? _invoice;

        public DetailVehicleInvoice(IInvoiceService invoiceService, int invoiceId)
        {
            InitializeComponent();
            _invoiceService = invoiceService;
            _=Initialize(invoiceId);
        }

        private async Task Initialize(int invoiceId)
        {
            VechicleInvoice invoice = await _invoiceService.GetVehicleInvoiceById(invoiceId);
            _invoice = invoice;
            VehicleDataGrid.ItemsSource = await _invoiceService.GetDetailVehicleInvoiceById(invoiceId);
            VehicleInvoiceIdInput.Text = invoice.VechicleInvoiceId.ToString();
            TotalAmountInput.Text = invoice.TotalAmount.ToString();
            ApartmentCodeInput.Text = invoice.Apartment.ApartmentCode;
            CreatedDateInput.Text = invoice.Invoice.CreatedDate.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
