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

        [ObservableProperty]
        private ObservableCollection<Equipment> brokenEquipments = new();

        [ObservableProperty]
        private ObservableCollection<MalfuntionEquipmentDTO> selectedEquipments = new();

        [ObservableProperty]
        private double totalRepairCost;


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
        private void AddBrokenEquipmentToInvoice(Equipment equipment)
        {
            var malfuntionEquipment = new MalfuntionEquipmentDTO
            {
                EquipmentId = equipment.EquipmentId,
                EquipmentName = equipment.EquipmentName,
                Description = "Mô tả hư hỏng",
                SolvingMethod = "Phương pháp sửa chữa",
                RepairPrice = 0
            };

            SelectedEquipments.Add(malfuntionEquipment);
            CalculateTotalRepairCost();
        }

        private void CalculateTotalRepairCost()
        {
            TotalRepairCost = SelectedEquipments.Sum(e => e.RepairPrice);
        }



        [RelayCommand]
        private async Task CreateInvoiceAsync()
        {
            // Tạo hóa đơn mới
            var invoice = new RepairInvoice
            {
                InvoiceContent = "Nội dung hóa đơn",
                TotalAmount = TotalRepairCost,
                Status = "Chưa thanh toán",
                //CreatedBy = CurrentUserId ?? throw new InvalidOperationException("User ID is not set.")
            };

            // Chuẩn bị danh sách thiết bị hỏng
            var malfunctionEquipments = SelectedEquipments.Select(equipment => new MalfuntionEquipment
            {
                EquipmentId = equipment.EquipmentId,
                Description = equipment.Description,
                SolvingMethod = equipment.SolvingMethod,
                RepairPrice = equipment.RepairPrice
            }).ToList();

            // Gửi tới Service để xử lý
            await _repairInvoiceService.AddRepairInvoiceWithDetailsAsync(invoice, malfunctionEquipments);

            // Làm mới giao diện
            SelectedEquipments.Clear();
            CalculateTotalRepairCost();
            await LoadRepairInvoicesAsync();
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
