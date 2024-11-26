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
        Task<ObservableCollection<CommunityRoomBookingDTO>> GetAllBookingsAsync();
        Task<bool> CreateBookingAsync(CommunityRoomBooking booking);
        Task<bool> UpdateBookingAsync(CommunityRoomBooking booking);
        Task<bool> DeleteBookingAsync(int id);
        Task<byte[]> GenerateBookingReceiptAsync(int bookingId);
        Task RegisterCommunityRoomAsync(CommunityRoomBookingDTO bookingDTO);
        Task<IEnumerable<CommunityRoomBookingDTO>> SearchBookingsAsync(string? apartmentCode, DateTime? bookingDate);
        Task<IEnumerable<ApartmentDTO>> GetAllApartmentsAsync();
    }
}
