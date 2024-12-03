﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class VehicleLimitDTO
    {
        public int MaxBicycles { get; set; } = 2;
        public int MaxMotorcycles { get; set; } = 2; // Including electric
        public int MaxCars { get; set; } = 1; // Including electric
        public int CurrentBicycles { get; set; }
        public int CurrentMotorcycles { get; set; }
        public int CurrentCars { get; set; }
    }
}
