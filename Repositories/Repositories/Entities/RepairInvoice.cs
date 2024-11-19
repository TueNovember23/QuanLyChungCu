using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class RepairInvoice
{
    public int InvoiceId { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public string? InvoiceContent { get; set; }

    public double TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public virtual Account? CreatedByNavigation { get; set; }

    public virtual ICollection<MalfuntionEquipment> MalfuntionEquipments { get; set; } = new List<MalfuntionEquipment>();
}
