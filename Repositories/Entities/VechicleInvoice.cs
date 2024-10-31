using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class VechicleInvoice
{
    public int VechicleInvoiceId { get; set; }

    public double? TotalAmount { get; set; }
}
