using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.RepairInvoiceDTO
{
    public class ResponseRepairInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceContent { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<MalfuntionEquipmentDTO> MalfuntionEquipments { get; set; } = new List<MalfuntionEquipmentDTO>();
    }
}
