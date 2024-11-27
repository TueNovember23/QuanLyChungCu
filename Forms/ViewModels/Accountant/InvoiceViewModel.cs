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
        private ObservableCollection<ResponseInvoiceDTO> invoices = [];

        [ObservableProperty]
        private ObservableCollection<ResponseInvoiceDTO> filteredInvoices = [];

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private int selectedMonth;

        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private bool isLoading;

        public InvoiceViewModel(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
            LoadInvoicesAsync();
            InitializeFilters();
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
                var invoiceList = await _invoiceService.GetAll();
                FilteredInvoices = Invoices = new ObservableCollection<ResponseInvoiceDTO>(invoiceList);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            SearchText = string.Empty;
            FilteredInvoices = new ObservableCollection<ResponseInvoiceDTO>(Invoices);
        }

        [RelayCommand]
        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredInvoices = new ObservableCollection<ResponseInvoiceDTO>(Invoices);
            }
            else
            {
                var result = await _invoiceService.Search(SearchText);
                FilteredInvoices = new ObservableCollection<ResponseInvoiceDTO>(result);
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

        [RelayCommand]
        private async Task SendInvoices()
        {
            IsLoading = true;
            try
            {
                await _invoiceService.SendInvoices(SelectedMonth, SelectedYear);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
