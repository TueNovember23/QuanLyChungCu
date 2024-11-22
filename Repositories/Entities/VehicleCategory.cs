﻿using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class VehicleCategory
{
    public int VehicleCategoryId { get; set; }

    public string? CategoryName { get; set; }

    public double? MonthlyFee { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
