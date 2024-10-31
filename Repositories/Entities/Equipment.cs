using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public string? Discription { get; set; }

    public int AreaId { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<MalfunctionReport> MalfunctionReports { get; set; } = new List<MalfunctionReport>();

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
}
