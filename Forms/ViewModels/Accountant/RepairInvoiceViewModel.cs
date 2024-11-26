using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Services.Services.AccountantServices;
using Services.DTOs.RepairInvoiceDTO;

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
        private void CreateInvoice()
        {
            // TODO: Implement create invoice logic
            System.Diagnostics.Debug.WriteLine("CreateInvoice called");
        }

        [RelayCommand]
        private async Task ViewInvoiceDetails(int invoiceId)
        {
            // TODO: Implement view invoice details logic
            System.Diagnostics.Debug.WriteLine($"ViewInvoiceDetails called for InvoiceId: {invoiceId}");
        }

    }
}
