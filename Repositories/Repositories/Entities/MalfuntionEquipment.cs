using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class MalfuntionEquipment
{
    public int MalfuntionId { get; set; }

    public int? RepairInvoiceId { get; set; }

    public int? EquipmentId { get; set; }

    public string? Description { get; set; }

    public string? SolvingMethod { get; set; }

    public double RepairPrice { get; set; }

    public virtual Equipment? Equipment { get; set; }

    public virtual RepairInvoice? RepairInvoice { get; set; }
}
