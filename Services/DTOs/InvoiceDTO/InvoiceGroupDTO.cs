using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.InvoiceDTO
{
    public class InvoiceGroupDTO
    {
        public List<ResponseInvoiceDTO> TotalInvoices { get; set; } = [];
        public List<ResponseWaterInvoiceDTO> WaterInvoices { get; set; } = [];
        public List<ResponseManagementFeeInvoiceDTO> ManagementInvoices { get; set; } = [];
        public List<ResponseVehicleInvoiceDTO> VehicleInvoices { get; set; } = [];
    }
}
