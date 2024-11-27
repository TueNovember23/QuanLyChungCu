using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.CommunityRoomBookingDTO
{
    public class ResponseCommunityRoomBookingDTO
    {
        public int BookingId { get; set; }
        public string ApartmentCode { get; set; } = null!;
        public DateOnly BookingDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public int NumberOfPeople { get; set; }
        public string RoomName { get; set; } = null!;
    }
}
