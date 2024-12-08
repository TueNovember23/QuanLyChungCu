using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.EquipmentDTO
{
    public class CreateRepairInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceContent { get; set; }
        public double TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public List<MalfunctionEquipmentDTO> MalfunctionEquipments { get; set; } = new();
    }

}
