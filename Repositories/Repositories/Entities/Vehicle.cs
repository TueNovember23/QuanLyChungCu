using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Vehicle
{
    public string VehicleId { get; set; } = null!;

    public string? VechicleOwner { get; set; }

    public int ApartmentId { get; set; }

    public int VehicleCategoryId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual VehicleCategory VehicleCategory { get; set; } = null!;
}
