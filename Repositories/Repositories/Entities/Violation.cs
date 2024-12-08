using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Violation
{
    public int ViolationId { get; set; }

    public int ApartmentId { get; set; }

    public int RegulationId { get; set; }

    public DateOnly CreatedDate { get; set; }

    public string? Detail { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual Regulation Regulation { get; set; } = null!;

    public virtual ICollection<ViolationPenalty> ViolationPenalties { get; set; } = new List<ViolationPenalty>();
}
