using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Maintenance
{
    public int MaintenanceId { get; set; }

    public string MaintenanceName { get; set; } = null!;

    public DateOnly MaintanaceDate { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Account CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
