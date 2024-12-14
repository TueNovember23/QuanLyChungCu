using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forms.Views.Accountant;
using Repositories.Repositories.Entities;
using Services.DTOs.EquipmentDTO;
using Services.DTOs.RepairInvoiceDTO;
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
        private ObservableCollection<ResponseRepairInvoiceDTO> repairInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseEquipmentDTO> availableEquipments = new();

        [ObservableProperty]
        private ObservableCollection<MalfunctionEquipmentDTO> selectedEquipments = new();

        [ObservableProperty]
        private ObservableCollection<ResponseRepairInvoiceDTO> filteredRepairInvoices = [];

        [ObservableProperty]
        private string invoiceContent;

        [ObservableProperty]
        private double totalAmount;

        [ObservableProperty]
        private string searchText;

        [RelayCommand]
        public void OnSearchButtonClick()
        {
            OnSearchTextChanged(); 
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
            var selectedEquipment = new MalfunctionEquipmentDTO
            {
                EquipmentId = equipment.EquipmentId,
                EquipmentName = equipment.EquipmentName,
            };

            SelectedEquipments.Add(selectedEquipment);
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

            TotalAmount = SelectedEquipments.Sum(e => e.RepairPrice);

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
                CloseForm();
                await LoadInvoicesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadInvoicesAsync()
        {
            var invoiceList = await _repairInvoiceService.GetAllRepairInvoicesAsync();
            FilteredRepairInvoices = RepairInvoices = new ObservableCollection<ResponseRepairInvoiceDTO>(invoiceList);
        }

        private void CloseForm()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = Application.Current.Windows.OfType<AddRepairInvoiceView>().FirstOrDefault();
                window?.Close();
            });
        }
    }
}