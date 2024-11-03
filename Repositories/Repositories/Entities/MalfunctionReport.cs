using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class MalfunctionReport
{
    public int ReportId { get; set; }

    public DateOnly ReportDate { get; set; }

    public string? ReportContent { get; set; }

    public string ReportStatus { get; set; } = null!;

    public int EquipmentId { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual ICollection<RepairInvoice> RepairInvoices { get; set; } = new List<RepairInvoice>();
}
