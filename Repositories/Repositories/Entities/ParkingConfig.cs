using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class ParkingConfig
{
    public int ConfigId { get; set; }

    public int? CategoryId { get; set; }

    public int MaxPerApartment { get; set; }

    public int TotalSpacePercent { get; set; }

    public virtual VehicleCategory? Category { get; set; }
}
