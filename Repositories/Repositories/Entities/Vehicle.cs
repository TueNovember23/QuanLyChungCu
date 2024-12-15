using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Vehicle
{
    public string VehicleId { get; set; } = null!;

    public string? VehicleOwner { get; set; }

    public string? Status { get; set; }

    public int ApartmentId { get; set; }

    public int VehicleCategoryId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual ICollection<VechicleInvoiceDetail> VechicleInvoiceDetails { get; set; } = new List<VechicleInvoiceDetail>();

    public virtual VehicleCategory VehicleCategory { get; set; } = null!;
}
