using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class VehicleDTO
    {
        public string VehicleNumber { get; set; } = null!;
        public string VehicleType { get; set; } = null!;
        public string VehicleOwner { get; set; } = null!;
        public int ApartmentId { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
