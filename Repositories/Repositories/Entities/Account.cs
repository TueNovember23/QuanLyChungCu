using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Role Role { get; set; } = null!;
}
