using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int NumberOfStaff { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();

    public virtual ICollection<RepairInvoice> RepairInvoices { get; set; } = new List<RepairInvoice>();
}
