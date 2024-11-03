using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class ManagementFeeInvoice
{
    public int ManagementFeeInvoiceId { get; set; }

    public double Area { get; set; }

    public double Price { get; set; }

    public double? TotalAmount { get; set; }

    public int InvoiceId { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
