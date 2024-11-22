using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class VehicleDTO
    {
        public string VehicleNumber { get; set; } = null!;  // Biển số xe
        public string VehicleType { get; set; } = null!;   // Loại xe
        public string VehicleOwner { get; set; } = null!;      // Tên chủ xe
        public string IdentityCard { get; set; } = string.Empty;
        public int ApartmentId { get; set; }       // ID căn hộ liên kết
    }
}
