using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public string? Discription { get; set; }

    public int AreaId { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<MalfuntionEquipment> MalfuntionEquipments { get; set; } = new List<MalfuntionEquipment>();

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
}
