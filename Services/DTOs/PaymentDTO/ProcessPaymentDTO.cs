using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.PaymentDTO
{
    public class ProcessPaymentDTO
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? Note { get; set; }
        public string Status { get; set; } = null!;
    }
}
