using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class VehicleLimitDTO
    {
        public int MaxBicycles { get; set; } = 1;
        public int MaxMotorcycles { get; set; } = 3;
        public int MaxCars { get; set; } = 1;
        public int CurrentBicycles { get; set; }
        public int CurrentMotorcycles { get; set; }
        public int CurrentCars { get; set; }
    }
}
