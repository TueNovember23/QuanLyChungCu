using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Regulation
{
    public int RegulationId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Category { get; set; }

    public string Priority { get; set; }

    public bool IsActive { get; set; }

    public DateOnly CreatedDate { get; set; }

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
