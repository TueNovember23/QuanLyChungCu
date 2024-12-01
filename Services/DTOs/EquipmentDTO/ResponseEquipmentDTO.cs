using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.EquipmentDTO
{
    public class ResponseEquipmentDTO
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; } = null!;
        public string? Discription { get; set; }
        public string AreaName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? LastMaintenanceDate { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastCheckDate { get; set; }
    }
}
