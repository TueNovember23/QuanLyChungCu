using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public double? TotalAmount { get; set; }

    public string? Status { get; set; }

    public virtual Account CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<VechicleInvoice> VechicleInvoices { get; set; } = new List<VechicleInvoice>();

    public virtual ICollection<WaterInvoice> WaterInvoices { get; set; } = new List<WaterInvoice>();

    public virtual ICollection<ManagementFeeInvoice> ManagementFeeInvoices { get; set; } = new List<ManagementFeeInvoice>();
}
