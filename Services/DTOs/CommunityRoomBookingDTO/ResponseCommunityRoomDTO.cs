using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.CommunityRoomBookingDTO
{
    public class ResponseCommunityRoomDTO
    {
        public int CommunityRoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public int RoomSize { get; set; }
        public string? Location { get; set; }
        public int? CurrentBookings { get; set; }
    }
}
