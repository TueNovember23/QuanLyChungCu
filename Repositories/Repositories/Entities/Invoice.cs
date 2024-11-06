using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public int? Month { get; set; }

    public int? Year { get; set; }

    public double? TotalAmount { get; set; }

    public string? Status { get; set; }

    public virtual Account CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<VechicleInvoice> VechicleInvoices { get; set; } = new List<VechicleInvoice>();
}
