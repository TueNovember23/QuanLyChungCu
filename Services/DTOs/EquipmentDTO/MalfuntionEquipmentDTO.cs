using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.EquipmentDTO
{
    public class MalfuntionEquipmentDTO
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string AreaName { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string SolvingMethod { get; set; }
        public decimal RepairPrice { get; set; }
    }
}
