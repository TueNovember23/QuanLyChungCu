using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Resident
{
    public string ResidentId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? RelationShipWithOwner { get; set; }

    public DateOnly? MoveInDate { get; set; }

    public DateOnly? MoveOutDate { get; set; }

    public int ApartmentId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual ICollection<HouseholdMovement> HouseholdMovements { get; set; } = new List<HouseholdMovement>();
}
