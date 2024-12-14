using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.Interfaces.AccountantServices;
using System.Collections.ObjectModel;
using Services.DTOs.RepairInvoiceDTO;
using System.Windows;
using Services.DTOs.EquipmentDTO;
using Services.Services.AccountantServices;
using Forms.Views.Accountant;

namespace Forms.ViewModels.Accountant
{
    public partial class RepairInvoiceViewModel : ObservableObject
    {
        private readonly IRepairInvoiceService _repairInvoiceService;
        private readonly IEquipmentService _equipmentService;

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

        [ObservableProperty]
        private ObservableCollection<Equipment> malfunctionEquipments = new();

        [ObservableProperty]
        private ObservableCollection<MalfunctionEquipmentDTO> selectedMalfunctions = new();

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

        private void OnAddRepairInvoiceClosed(object sender, EventArgs e)
        {
            LoadRepairInvoicesAsync();
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadRepairInvoicesAsync();
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
        private void AddEquipmentToInvoice(Equipment equipment)
        {
            var dto = new MalfunctionEquipmentDTO
            {
                EquipmentId = equipment.EquipmentId,
                EquipmentName = equipment.EquipmentName
            };
            SelectedMalfunctions.Add(dto);
        }


        [RelayCommand]
        private async Task ViewInvoiceDetails(int invoiceId)
        {
            try
            {
                var invoiceDetailView = new InvoiceDetailView();
                var viewModel = new InvoiceDetailViewModel(_repairInvoiceService, invoiceId);
                invoiceDetailView.DataContext = viewModel;
                invoiceDetailView.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xem chi tiết hóa đơn: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task OpenAddRepairInvoiceView()
        {
            int nextInvoiceCode = await _repairInvoiceService.GenerateNewRepairInvoiceCodeAsync();

            var addRepairInvoiceView = new AddRepairInvoiceView(_repairInvoiceService);
            addRepairInvoiceView.ShowDialog();
        }


    }
}
