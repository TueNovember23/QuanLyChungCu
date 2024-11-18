using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class WaterInvoice
{
    public int WaterInvoiceId { get; set; }

    public int StartIndex { get; set; }

    public int EndIndex { get; set; }

    public int Level { get; set; }

    public double Price { get; set; }

    public double? TotalAmount { get; set; }

    public int InvoiceId { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
