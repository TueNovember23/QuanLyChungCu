using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class WaterInvoice
{
    public int WaterInvoiceId { get; set; }

    public int StartIndex { get; set; }

    public int EndIndex { get; set; }

    public double? TotalAmount { get; set; }

    public int InvoiceId { get; set; }

    public int ApartmentId { get; set; }

    public int WaterFeeId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual WaterFee WaterFee { get; set; } = null!;
}
