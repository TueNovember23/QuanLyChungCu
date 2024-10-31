using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class CommunityRoomBooking
{
    public int CommunityRoomBookingId { get; set; }

    public DateOnly BookingDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int NumberOfPeople { get; set; }

    public int ApartmentId { get; set; }

    public int CommunityRoomId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual CommunityRoom CommunityRoom { get; set; } = null!;
}
