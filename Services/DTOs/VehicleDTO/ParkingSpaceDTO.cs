using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class ParkingSpaceDTO
    {
        public string VehicleType { get; set; } = string.Empty;
        public int MaxPerApartment { get; set; }
        public int TotalSpaces { get; set; }
        public int UsedSpaces { get; set; }
        public int AvailableSpaces => TotalSpaces - UsedSpaces;
    }
}
