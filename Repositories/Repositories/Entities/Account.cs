using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Account
{
    public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CommunityRoomBooking> CommunityRoomBookings { get; set; } = new List<CommunityRoomBooking>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();

    public virtual ICollection<RepairInvoice> RepairInvoices { get; set; } = new List<RepairInvoice>();

    public virtual Role Role { get; set; } = null!;
}
