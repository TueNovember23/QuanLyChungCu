using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class VehicleUpdateDTO
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
