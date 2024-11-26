using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.CommunityRoomBookingDTO
{
    public class CommunityRoomBookingDTO
    {
        public int BookingId { get; set; }
        public string ApartmentCode { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public int NumberOfPeople { get; set; }
        public string RoomName { get; set; }
    }
}
