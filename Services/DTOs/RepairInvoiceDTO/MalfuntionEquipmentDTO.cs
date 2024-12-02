using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.RepairInvoiceDTO
{
    public class MalfuntionEquipmentDTO
    {
        public int MalfuntionId { get; set; }
        public int? RepairInvoiceId { get; set; }
        public int? EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
        public string? Description { get; set; }
        public string? SolvingMethod { get; set; }
        public double RepairPrice { get; set; }
    }

}
