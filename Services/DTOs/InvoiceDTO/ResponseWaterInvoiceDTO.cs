using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.InvoiceDTO
{
    public class ResponseWaterInvoiceDTO
    {
        public int WaterInvoiceId { get; set; }
        public string ApartmentCode { get; set; } = null!;
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int NumberOfPeople { get; set; }
        public double TotalAmount { get; set; }
        public DateOnly CreatedDate { get; set; }
        public int InvoiceId { get; set; }
    }
}
