using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.Interfaces.AccountantServices;
using System.Collections.ObjectModel;
using Services.DTOs.RepairInvoiceDTO;
using System.Windows;

namespace Forms.ViewModels.Accountant
{
    public partial class RepairInvoiceViewModel : ObservableObject
    {
        private readonly IRepairInvoiceService _repairInvoiceService;

        [ObservableProperty]
        private ObservableCollection<ResponseRepairInvoiceDTO> repairInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseRepairInvoiceDTO> filteredRepairInvoices = [];

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private string newInvoiceContent;

        [ObservableProperty]
        private double newInvoiceTotalAmount;


        public RepairInvoiceViewModel(IRepairInvoiceService repairInvoiceService)
        {
            _repairInvoiceService = repairInvoiceService;
            _ = LoadRepairInvoicesAsync();
        }

        private async Task LoadRepairInvoicesAsync()
        {
            var invoiceList = await _repairInvoiceService.GetAllRepairInvoicesAsync();
            FilteredRepairInvoices = RepairInvoices = new ObservableCollection<ResponseRepairInvoiceDTO>(invoiceList);
        }

        [RelayCommand]
        private void Refresh()
        {
            SearchText = string.Empty;
            FilteredRepairInvoices = new ObservableCollection<ResponseRepairInvoiceDTO>(RepairInvoices);
        }

        [RelayCommand]
        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredRepairInvoices = new ObservableCollection<ResponseRepairInvoiceDTO>(RepairInvoices);
            }
            else
            {
                var result = await _repairInvoiceService.SearchRepairInvoicesAsync(SearchText);
                FilteredRepairInvoices = new ObservableCollection<ResponseRepairInvoiceDTO>(result);
            }
        }

        [RelayCommand]
        private async Task CreateInvoice()
        {
            // Dữ liệu từ form
            var newInvoice = new RepairInvoice
            {
                InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
                InvoiceContent = NewInvoiceContent,
                TotalAmount = NewInvoiceTotalAmount,
                Status = "Đang xử lý", // Mặc định
                //CreatedBy = CurrentUserId
            };

            try
            {
                await _repairInvoiceService.AddRepairInvoiceAsync(newInvoice);
                MessageBox.Show("Hóa đơn sửa chữa được tạo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadRepairInvoicesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo hóa đơn sửa chữa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        [RelayCommand]
        private async Task ViewInvoiceDetails(int invoiceId)
        {
            var invoice = await _repairInvoiceService.GetRepairInvoiceByIdAsync(invoiceId);
            if (invoice != null)
            {
                // Hiển thị chi tiết hóa đơn
                MessageBox.Show(
                    $"Mã hóa đơn: {invoice.InvoiceId}\n" +
                    $"Nội dung: {invoice.InvoiceContent}\n" +
                    $"Tổng tiền: {invoice.TotalAmount:N0} VND\n" +
                    $"Trạng thái: {invoice.Status}\n" +
                    $"Ngày lập: {invoice.InvoiceDate:dd/MM/yyyy}",
                    "Chi tiết hóa đơn",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                MessageBox.Show("Không tìm thấy hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
