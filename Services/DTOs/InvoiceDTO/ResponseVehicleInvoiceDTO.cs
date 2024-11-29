using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.InvoiceDTO
{
    public class ResponseVehicleInvoiceDTO
    {
        public int VehicleInvoiceId { get; set; }
        public string ApartmentCode { get; set; } = null!;
        public double TotalAmount { get; set; }
        public DateOnly CreatedDate { get; set; }
        public int InvoiceId { get; set; }
        public List<VehicleDetailDTO> Details { get; set; } = [];
    }
}
