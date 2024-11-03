namespace Repositories.Entities;

public partial class Apartment
{
    public int ApartmentId { get; set; }

    public int? Area { get; set; }

    public string? Status { get; set; }

    public int FloorId { get; set; }

    public virtual ICollection<CommunityRoomBooking> CommunityRoomBookings { get; set; } = new List<CommunityRoomBooking>();

    public virtual Floor Floor { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Representative> Representatives { get; set; } = new List<Representative>();

    public virtual ICollection<Resident> Residents { get; set; } = new List<Resident>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
