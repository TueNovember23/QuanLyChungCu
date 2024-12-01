using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.CommunityRoomBookingDTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.ServiceSupervisorServices
{
    public interface ICommunityRoomService
    {
        Task<List<ResponseCommunityRoomDTO>> GetAll();
        Task<List<ResponseCommunityRoomDTO>> Search(string searchText);
        Task<List<ResponseCommunityRoomBookingDTO>> GetBookings();
        Task<bool> CreateBooking(int communityRoomId, int apartmentId, DateOnly bookingDate,
            TimeOnly startTime, TimeOnly endTime, int numberOfPeople);
        Task<bool> DeleteBooking(int bookingId);
        Task<bool> IsRoomAvailable(int communityRoomId, DateOnly bookingDate,
       TimeOnly startTime, TimeOnly endTime);
    }
}
