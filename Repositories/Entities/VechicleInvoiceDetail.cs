using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class VechicleInvoiceDetail
{
    public int VechicleInvoiceId { get; set; }

    public string VehicleId { get; set; } = null!;

    public double Price { get; set; }

    public virtual VechicleInvoice VechicleInvoice { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
