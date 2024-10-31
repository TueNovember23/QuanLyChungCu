using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class CommunityRoom
{
    public int CommunityRoomId { get; set; }

    public string RoomName { get; set; } = null!;

    public int RoomSize { get; set; }

    public string? Location { get; set; }

    public virtual ICollection<CommunityRoomBooking> CommunityRoomBookings { get; set; } = new List<CommunityRoomBooking>();
}
