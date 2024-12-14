using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.PaymentDTO
{
    public class ResponsePaymentDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; } = null!;
        public string ApartmentCode { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = null!;
        public string Type { get; set; } = "Invoice"; 
    }
}
