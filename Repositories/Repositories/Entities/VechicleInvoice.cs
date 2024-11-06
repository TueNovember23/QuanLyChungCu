using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class VechicleInvoice
{
    public int VechicleInvoiceId { get; set; }

    public double? TotalAmount { get; set; }

    public int InvoiceId { get; set; }

    public int ApartmentId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;
}
