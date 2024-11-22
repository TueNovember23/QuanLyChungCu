using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class Floor
{
    public int FloorId { get; set; }

    public int FloorNumber { get; set; }

    public bool? IsDeleted { get; set; }

    public int BlockId { get; set; }

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

    public virtual Block Block { get; set; } = null!;
}
