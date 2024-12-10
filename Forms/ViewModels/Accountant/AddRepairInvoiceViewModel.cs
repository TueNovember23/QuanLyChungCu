using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.Accountant;
using Services.DTOs.EquipmentDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forms.ViewModels.Accountant
{
    public partial class AddRepairInvoiceViewModel : ObservableObject
    {
        private readonly IRepairInvoiceService _repairInvoiceService;
        [ObservableProperty]
        private int repairInvoiceCode;


        [ObservableProperty]
        private ObservableCollection<ResponseEquipmentDTO> availableEquipments = new();

        [ObservableProperty]
        private ObservableCollection<MalfunctionEquipmentDTO> selectedEquipments = new();

        [ObservableProperty]
        private string invoiceContent;

        [ObservableProperty]
        private double totalAmount;

        [ObservableProperty]
        private string searchText;

        [RelayCommand]
        public void OnSearchButtonClick()
        {
            OnSearchTextChanged();  // Gọi lại phương thức tìm kiếm khi button được click
        }

        public AddRepairInvoiceViewModel(IRepairInvoiceService repairInvoiceService)
        {
            _repairInvoiceService = repairInvoiceService;
            RepairInvoiceCode = repairInvoiceCode;
            InitializeInvoiceCode();

            LoadEquipments();
        }

        private async void InitializeInvoiceCode()
        {
            RepairInvoiceCode = await _repairInvoiceService.GenerateNewRepairInvoiceCodeAsync();
            if (RepairInvoiceCode <= 0)
            {
                MessageBox.Show("Lỗi khi tạo mã hóa đơn mới!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void LoadEquipments()
        {
            var equipments = await _repairInvoiceService.GetAvailableEquipmentsAsync();
            AvailableEquipments = new ObservableCollection<ResponseEquipmentDTO>(equipments);

        }
        [RelayCommand]
        public void OnSearchTextChanged()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadEquipments();
            }
            else
            {
                var filteredEquipments = AvailableEquipments
                    .Where(e => e.EquipmentName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                e.EquipmentId.ToString().Contains(SearchText))
                    .ToList();
                AvailableEquipments = new ObservableCollection<ResponseEquipmentDTO>(filteredEquipments);
            }
        }


        [RelayCommand]
        private async void AddEquipment(ResponseEquipmentDTO equipment)
        {
            if (SelectedEquipments.Any(e => e.EquipmentId == equipment.EquipmentId))
            {
                return;
            }

            var malfunctionEquipment = await _repairInvoiceService.GetMalfunctionEquipmentByIdAsync(equipment.EquipmentId);

            if (malfunctionEquipment != null)
            {
                var malfunction = new MalfunctionEquipmentDTO
                {
                    EquipmentId = malfunctionEquipment.EquipmentId ?? 0,
                    EquipmentName = malfunctionEquipment.EquipmentName,
                    RepairPrice = malfunctionEquipment.RepairPrice
                };

                SelectedEquipments.Add(malfunction);
                TotalAmount = SelectedEquipments.Sum(e => e.RepairPrice);
            }
        }

        [RelayCommand]
        private void RemoveEquipment(MalfunctionEquipmentDTO equipment)
        {
            if (SelectedEquipments.Contains(equipment))
            {
                SelectedEquipments.Remove(equipment);
                TotalAmount = SelectedEquipments.Sum(e => e.RepairPrice);
            }
        }

        [RelayCommand]
        private async Task SaveInvoiceAsync()
        {
            if (string.IsNullOrWhiteSpace(InvoiceContent) || !SelectedEquipments.Any())
            {
                MessageBox.Show("Vui lòng nhập nội dung hóa đơn và chọn ít nhất một thiết bị!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var invoice = new CreateRepairInvoiceDTO
            {
                InvoiceId = RepairInvoiceCode,
                InvoiceContent = InvoiceContent,
                TotalAmount = TotalAmount,
                MalfunctionEquipments = SelectedEquipments.ToList()
            };

            try
            {
                await _repairInvoiceService.AddRepairInvoiceAsync(invoice);
                MessageBox.Show("Lưu hóa đơn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}