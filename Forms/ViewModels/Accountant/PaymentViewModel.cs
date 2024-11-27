using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.DTOs.PaymentDTO;
using Services.Interfaces.AccountantServices;
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
        private ObservableCollection<PaymentHistoryDTO> paymentHistory = [];

        public ObservableCollection<int> Months { get; } = new(Enumerable.Range(1, 12));
        public ObservableCollection<int> Years { get; } = new(Enumerable.Range(2020, 10));
        public ObservableCollection<string> PaymentStatuses { get; } = ["Tất cả", "Chưa thanh toán", "Đã thanh toán một phần", "Đã thanh toán"];
        public ObservableCollection<string> PaymentMethods { get; } = ["Tiền mặt", "Chuyển khoản"];

        public PaymentViewModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
            selectedMonth = DateTime.Now.Month;
            selectedYear = DateTime.Now.Year;
            selectedStatus = PaymentStatuses[0];
            _ = LoadPaymentsAsync();
        }

        private async Task LoadPaymentsAsync()
        {
            var paymentList = await _paymentService.GetPayments(selectedMonth, selectedYear, selectedStatus);
            Payments = new ObservableCollection<ResponsePaymentDTO>(paymentList);
        }

        [RelayCommand]
        private async Task Refresh()
        {
            SearchText = string.Empty;
            await LoadPaymentsAsync();
        }

        [RelayCommand]
        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadPaymentsAsync();
            }
            else
            {
                var result = await _paymentService.SearchPayments(SearchText, selectedMonth, selectedYear, selectedStatus);
                Payments = new ObservableCollection<ResponsePaymentDTO>(result);
            }
        }

        [RelayCommand]
        private async Task ProcessPayment()
        {
            if (SelectedPayment == null || PaymentAmount <= 0 || string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                // Show error message
                return;
            }

            var paymentRequest = new ProcessPaymentDTO
            {
                InvoiceId = SelectedPayment.InvoiceId,
                Amount = PaymentAmount,
                PaymentMethod = SelectedPaymentMethod,
                Note = PaymentNote
            };

            await _paymentService.ProcessPayment(paymentRequest);
            await LoadPaymentsAsync();
            await LoadPaymentHistory(SelectedPayment.InvoiceId);
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
