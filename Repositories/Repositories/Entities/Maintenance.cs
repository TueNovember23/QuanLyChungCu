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

    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
