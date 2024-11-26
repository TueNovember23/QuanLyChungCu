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

namespace Forms.ViewModels.Accountant
{
    public partial class RepairInvoiceViewModel : ObservableObject
    {
        private readonly IRepairInvoiceService _repairInvoiceService;

        private ObservableCollection<RepairInvoice> _repairInvoices;
        public ObservableCollection<RepairInvoice> RepairInvoices
        {
            get => _repairInvoices;
            set => SetProperty(ref _repairInvoices, value);
        }

        private int _totalInvoiceCount;
        public int TotalInvoiceCount
        {
            get => _totalInvoiceCount;
            set => SetProperty(ref _totalInvoiceCount, value);
        }

        private double _totalRepairCost;
        public double TotalRepairCost
        {
            get => _totalRepairCost;
            set => SetProperty(ref _totalRepairCost, value);
        }

        public RepairInvoiceViewModel(IRepairInvoiceService repairInvoiceService)
        {
            _repairInvoiceService = repairInvoiceService;
            LoadRepairInvoicesCommand = new AsyncRelayCommand(LoadRepairInvoicesAsync);
            RepairInvoices = new ObservableCollection<RepairInvoice>();
        }

        public ICommand LoadRepairInvoicesCommand { get; }

        private async Task LoadRepairInvoicesAsync()
        {
            var invoices = await _repairInvoiceService.GetAllRepairInvoicesAsync();
            RepairInvoices = new ObservableCollection<RepairInvoice>(invoices);
            TotalInvoiceCount = RepairInvoices.Count;
            TotalRepairCost = RepairInvoices.Sum(i => i.TotalAmount);
        }
    }

}
