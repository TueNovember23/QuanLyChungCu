using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Role Role { get; set; } = null!;
}
