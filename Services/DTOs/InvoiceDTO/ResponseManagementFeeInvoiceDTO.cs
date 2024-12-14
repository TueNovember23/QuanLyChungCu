using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.InvoiceDTO
{
    public class ResponseManagementFeeInvoiceDTO
    {
        public int ManagementFeeInvoiceId { get; set; }
        public string ApartmentCode { get; set; } = null!;
        public double? Fee { get; set; }
        public int? Area { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
        public DateOnly CreatedDate { get; set; }
        public int InvoiceId { get; set; }
    }
}
