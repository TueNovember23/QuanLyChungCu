using CommunityToolkit.Mvvm.ComponentModel;
using Services.DTOs.RepairInvoiceDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.ObjectModel;

namespace Forms.ViewModels.Accountant
{
    public partial class InvoiceDetailViewModel : ObservableObject
    {
        private readonly IRepairInvoiceService _repairInvoiceService;

        [ObservableProperty]
        private int invoiceId;

        [ObservableProperty]
        private string invoiceContent;

        [ObservableProperty]
        private decimal totalAmount;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private DateTime invoiceDate;

        [ObservableProperty]
        private ObservableCollection<MalfuntionEquipmentDTO> malfunctionEquipments;

        public InvoiceDetailViewModel(IRepairInvoiceService repairInvoiceService, int invoiceId)
        {
            _repairInvoiceService = repairInvoiceService;
            InvoiceId = invoiceId;
            MalfunctionEquipments = new ObservableCollection<MalfuntionEquipmentDTO>();
            LoadInvoiceDetailsAsync();
        }

        private async void LoadInvoiceDetailsAsync()
        {
            try
            {
                var invoice = await _repairInvoiceService.GetRepairInvoiceByIdAsync(InvoiceId);
                if (invoice != null)
                {
                    InvoiceContent = invoice.InvoiceContent;
                    TotalAmount = invoice.TotalAmount;
                    Status = invoice.Status;
                    InvoiceDate = invoice.InvoiceDate;

                    MalfunctionEquipments.Clear();
                    if (invoice.MalfuntionEquipments != null)
                    {
                        foreach (var equipment in invoice.MalfuntionEquipments)
                        {
                            MalfunctionEquipments.Add(new MalfuntionEquipmentDTO
                            {
                                EquipmentId = equipment.EquipmentId,
                                EquipmentName = equipment.EquipmentName,
                                Description = equipment.Description,
                                SolvingMethod = equipment.SolvingMethod,
                                RepairPrice = equipment.RepairPrice
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải chi tiết hóa đơn: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}