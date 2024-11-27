using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class VehicleStatisticsDTO
    {
        public int BicycleCount { get; set; }
        public int MotorcycleCount { get; set; }
        public int CarCount { get; set; }
    }
}
