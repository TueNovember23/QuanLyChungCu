﻿using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Windows.Media;

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
        private Apartment selectedApartment;

        [ObservableProperty]
        private bool isLoading;
        [ObservableProperty]
        private ObservableCollection<int> months = new ObservableCollection<int>();
        [ObservableProperty]
        private ObservableCollection<int> years = new ObservableCollection<int>();
        [ObservableProperty]
        private ObservableCollection<Apartment> apartments = new ObservableCollection<Apartment>();


        [ObservableProperty]
        private int startIndex;
        [ObservableProperty]
        private int endIndex;
        [ObservableProperty]
        private int numberOfPeople;
        [ObservableProperty]
        private double apartmentArea;
        [ObservableProperty]
        private double managementFeePrice;

        [ObservableProperty]
        private string apartmentCode;
        [ObservableProperty]
        private int invoiceId;

        public InvoiceViewModel(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
            InitializeFilters();
            LoadInvoicesAsync();
            LoadComboBoxData();
        }

        private async Task LoadComboBoxData()
        {
            try
            {
                for (int i = 1; i <= 12; i++)
                {
                    Months.Add(i);
                }

                int currentYear = DateTime.Now.Year;
                for (int i = 2020; i <= currentYear; i++)
                {
                    Years.Add(i);
                }

                await LoadApartmentsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading combo box data: {ex.Message}");
            }
        }



        private async Task LoadApartmentsAsync()
        {
            var apartmentsList = await _invoiceService.GetApartmentsAsync();
            Apartments = new ObservableCollection<Apartment>(apartmentsList);

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
                var invoiceInputs = new List<InvoiceInputDTO>(); 
                await _invoiceService.GenerateInvoices(SelectedMonth, SelectedYear, invoiceInputs);
                await LoadInvoicesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateWaterInvoice()
        {
            IsLoading = true;
            try
            {
                await _invoiceService.GenerateWaterInvoices(invoiceId, new Apartment { ApartmentCode = apartmentCode }, startIndex, endIndex, numberOfPeople);
                await LoadInvoicesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateManagementFeeInvoice()
        {
            IsLoading = true;
            try
            {
                await _invoiceService.GenerateManagementFeeInvoices(invoiceId, new Apartment { ApartmentCode = apartmentCode }, apartmentArea, managementFeePrice);
                await LoadInvoicesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateVehicleInvoice()
        {
            IsLoading = true;
            try
            {
                await _invoiceService.GenerateVehicleInvoices(invoiceId, new Apartment { ApartmentCode = apartmentCode });
                await LoadInvoicesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }


    }
}
