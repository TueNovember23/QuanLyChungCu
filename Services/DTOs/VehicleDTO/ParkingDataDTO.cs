using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class ParkingDataDTO
    {
        public List<ParkingSpaceDTO> Spaces { get; set; } = new();
        public VehicleLimitDTO Limits { get; set; } = new();
    }
}
