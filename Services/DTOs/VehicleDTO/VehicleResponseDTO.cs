using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class VehicleResponseDTO
    {
        public string VehicleNumber { get; set; } = null!;
        public string VehicleType { get; set; } = null!;
        public string VehicleOwner { get; set; } = null!;
        public string Status { get; set; } = null!;  // Trạng thái xe
        public float MonthlyFee { get; set; }
    }
}
