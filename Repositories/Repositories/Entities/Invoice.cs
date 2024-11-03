using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public double? TotalAmount { get; set; }

    public int ApartmentId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual ICollection<VechicleInvoice> VechicleInvoices { get; set; } = new List<VechicleInvoice>();
}
