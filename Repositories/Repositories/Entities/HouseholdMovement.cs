using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class HouseholdMovement
{
    public int MovementId { get; set; }

    public int ApartmentId { get; set; }

    public string ResidentId { get; set; } = null!;

    public DateOnly MovementDate { get; set; }

    public string MovementType { get; set; } = null!;

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual Resident Resident { get; set; } = null!;
}
