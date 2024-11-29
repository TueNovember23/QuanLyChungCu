using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Repositories.Repositories.Entities;
using Services.DTOs.InvoiceDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.ViewModels.Accountant
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly IInvoiceService _invoiceService;

        [ObservableProperty]
        private ObservableCollection<ResponseInvoiceDTO> totalInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseWaterInvoiceDTO> waterInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseManagementFeeInvoiceDTO> managementInvoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseVehicleInvoiceDTO> vehicleInvoices = [];

        [ObservableProperty]
        private int selectedMonth;

        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private bool isLoading;

        public InvoiceViewModel(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
            InitializeFilters();
            LoadInvoicesAsync();
        }

        private void InitializeFilters()
        {
            SelectedMonth = DateTime.Now.Month;
            SelectedYear = DateTime.Now.Year;
        }

        private async Task LoadInvoicesAsync()
        {
            IsLoading = true;
            try
            {
                var invoices = await _invoiceService.GetAllInvoices(SelectedMonth, SelectedYear);
                TotalInvoices = new ObservableCollection<ResponseInvoiceDTO>(invoices.TotalInvoices);
                WaterInvoices = new ObservableCollection<ResponseWaterInvoiceDTO>(invoices.WaterInvoices);
                ManagementInvoices = new ObservableCollection<ResponseManagementFeeInvoiceDTO>(invoices.ManagementInvoices);
                VehicleInvoices = new ObservableCollection<ResponseVehicleInvoiceDTO>(invoices.VehicleInvoices);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task GenerateInvoices()
        {
            IsLoading = true;
            try
            {
                await _invoiceService.GenerateInvoices(SelectedMonth, SelectedYear);
                await LoadInvoicesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
