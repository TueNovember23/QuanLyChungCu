using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class RepairInvoice
{
    public int InvoiceId { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public string? InvoiceContent { get; set; }

    public string? SolvingMethod { get; set; }

    public double RepairPrice { get; set; }

    public string Status { get; set; } = null!;

    public int? MalfunctionReportId { get; set; }

    public int? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual MalfunctionReport? MalfunctionReport { get; set; }
}
