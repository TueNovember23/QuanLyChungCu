using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Representative
{
    public string RepresentativeId { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}
