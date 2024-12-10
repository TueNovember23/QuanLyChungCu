using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.DTOs.InvoiceDTO;
using Services.DTOs.PaymentDTO;
using Services.Interfaces.AccountantServices;
using Services.Services.AccountantServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.ViewModels.Accountant
{
    public partial class PaymentViewModel : ObservableObject
    {
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceService _invoiceService;

        [ObservableProperty]
        private ObservableCollection<ResponseInvoiceDTO> totalInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponsePaymentDTO> payments = [];

        [ObservableProperty]
        private ResponsePaymentDTO? selectedPayment;

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private int selectedMonth;

        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private string selectedStatus = "";

        [ObservableProperty]
        private decimal paymentAmount;

        [ObservableProperty]
        private string selectedPaymentMethod = "";

        [ObservableProperty]
        private string paymentNote = "";

        [ObservableProperty]
        private string selectedPaymentStatus = "";

        [ObservableProperty]
        private ObservableCollection<string> apartments = new();

        [ObservableProperty]
        private string selectedApartment; 

        [ObservableProperty]
        private ObservableCollection<PaymentHistoryDTO> paymentHistory = [];

        public ObservableCollection<int> Months { get; } = new(Enumerable.Range(1, 12));
        public ObservableCollection<int> Years { get; } = new(Enumerable.Range(2020, 10));
        public ObservableCollection<string> PaymentStatuses { get; } = ["Tất cả", "Chưa thanh toán", "Đã thanh toán"];

        public PaymentViewModel(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
            selectedMonth = DateTime.Now.Month;
            selectedYear = DateTime.Now.Year;
            selectedStatus = PaymentStatuses[0];
            selectedPaymentStatus = "Chưa thanh toán";
            _ = LoadLoadLoad();
        }

        private async Task LoadLoadLoad()
        {
            await LoadInvoicesAsync();
            await LoadApartmentsAsync();
        }

        private async Task LoadInvoicesAsync()
        {
            var invoiceGroup = await _invoiceService.GetAllInvoices(SelectedMonth, SelectedYear);
            var invoices = invoiceGroup;
            TotalInvoices = new ObservableCollection<ResponseInvoiceDTO>(invoices);
        }

        private async Task LoadApartmentsAsync()
        {
            var apartmentsList = await _invoiceService.GetApartmentsAsync(); 
            Apartments = new ObservableCollection<string>(apartmentsList.Select(a => a.ApartmentCode));
        }

        [RelayCommand]
        private async Task Refresh()
        {
            SearchText = string.Empty;
            await LoadInvoicesAsync();
        }

        [RelayCommand]
        private async Task SearchInvoices()
        {
            var invoiceGroup = await _invoiceService.GetAllInvoices(SelectedMonth, SelectedYear);
            var invoices = invoiceGroup;
            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(SelectedStatus) && SelectedStatus != "Tất cả")
            {
                invoices = invoices.Where(i => i.Status == SelectedStatus).ToList();
            }

            // Lọc theo căn hộ
            if (!string.IsNullOrWhiteSpace(SelectedApartment))
            {
                invoices = invoices.Where(i => i.ApartmentCode == SelectedApartment).ToList();
            }


            TotalInvoices = new ObservableCollection<ResponseInvoiceDTO>(invoices);
        }

        [RelayCommand]
        private async Task UpdateInvoiceStatus(ResponseInvoiceDTO invoice)
        {
            if (invoice == null) return;

            // Chuyển trạng thái hóa đơn
            invoice.Status = invoice.Status == "Chưa thanh toán" ? "Đã thanh toán" : "Chưa thanh toán";
            await _invoiceService.UpdateInvoiceStatus(invoice.InvoiceId, invoice.Status);
            await LoadInvoicesAsync(); // Tải lại danh sách hóa đơn
        }


        [RelayCommand]
        private async Task PrintReceipt()
        {
            if (SelectedPayment == null) return;
            await _paymentService.GenerateReceipt(SelectedPayment.InvoiceId);
        }

        private async Task LoadPaymentHistory(int invoiceId)
        {
            var history = await _paymentService.GetPaymentHistory(invoiceId);
            PaymentHistory = new ObservableCollection<PaymentHistoryDTO>(history);
        }

        partial void OnSelectedPaymentChanged(ResponsePaymentDTO? value)
        {
            if (value != null)
            {
                _ = LoadPaymentHistory(value.InvoiceId);
            }
        }
    }
}
