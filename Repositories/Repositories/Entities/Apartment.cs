using System;
using System.Collections.Generic;

namespace Repositories.Repositories.Entities;

public partial class Apartment
{
    public int ApartmentId { get; set; }

    public string? ApartmentCode { get; set; }

    public int? ApartmentNumber { get; set; }

    public int? Area { get; set; }

    public int? NumberOfPeople { get; set; }

    public string? Status { get; set; }

    public int FloorId { get; set; }

    public virtual ICollection<CommunityRoomBooking> CommunityRoomBookings { get; set; } = new List<CommunityRoomBooking>();

    public virtual Floor Floor { get; set; } = null!;

    public virtual ICollection<Representative> Representatives { get; set; } = new List<Representative>();

    public virtual ICollection<Resident> Residents { get; set; } = new List<Resident>();

    public virtual ICollection<VechicleInvoice> VechicleInvoices { get; set; } = new List<VechicleInvoice>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();

    public virtual ICollection<WaterInvoice> WaterInvoices { get; set; } = new List<WaterInvoice>();
}
