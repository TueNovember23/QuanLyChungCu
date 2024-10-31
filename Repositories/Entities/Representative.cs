using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class Representative
{
    public string RepresentativeId { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int ApartmentId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;
}
