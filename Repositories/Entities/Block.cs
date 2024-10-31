using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class Block
{
    public int BlockId { get; set; }

    public string? BlockCode { get; set; }

    public int AreaId { get; set; }

    public bool? IsDeleleted { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
