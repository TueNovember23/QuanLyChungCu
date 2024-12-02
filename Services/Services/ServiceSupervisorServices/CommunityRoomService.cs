using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.CommunityRoomBookingDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System.Net.Http;

namespace Services.Services.ServiceSupervisorServices
{
    public class CommunityRoomService : ICommunityRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommunityRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
            
        public async Task<List<ResponseCommunityRoomDTO>> GetAll()
        {
            var rooms = await _unitOfWork.GetRepository<CommunityRoom>().Entities
                .Include(r => r.CommunityRoomBookings)
                .Select(room => new ResponseCommunityRoomDTO
                {
                    CommunityRoomId = room.CommunityRoomId,
                    RoomName = room.RoomName,
                    RoomSize = room.RoomSize,
                    Location = room.Location,
                    CurrentBookings = room.CommunityRoomBookings.Count
                }).ToListAsync();
            return rooms;
        }

        public async Task<List<ApartmentDTO>> GetApartments()
        {
            return await _unitOfWork.GetRepository<Apartment>()
                .Entities
                .Select(apartment => new ApartmentDTO
                {
                    ApartmentId = apartment.ApartmentId,
                    ApartmentCode = apartment.ApartmentCode,
                    Status = apartment.Status,
                    FloorNumber = apartment.Floor.FloorNumber,
                    OwnerName = apartment.Representative.FullName
                })
                .ToListAsync();
        }



        public async Task<List<ResponseCommunityRoomBookingDTO>> SearchBookings(string apartmentCode, DateOnly? bookingDate)
        {
            var query = _unitOfWork.GetRepository<CommunityRoomBooking>().Entities
              .Include(b => b.Apartment)
              .Include(b => b.CommunityRoom) as IQueryable<CommunityRoomBooking>;


            if (!string.IsNullOrEmpty(apartmentCode))
            {
                query = query.Where(b => b.Apartment.ApartmentCode.Contains(apartmentCode));
            }

            if (bookingDate.HasValue)
            {
                query = query.Where(b => b.BookingDate == bookingDate.Value);
            }

            var bookings = await query.Select(booking => new ResponseCommunityRoomBookingDTO
            {
                BookingId = booking.CommunityRoomBookingId,
                ApartmentCode = booking.Apartment.ApartmentCode!,
                BookingDate = booking.BookingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                NumberOfPeople = booking.NumberOfPeople,
                RoomName = booking.CommunityRoom.RoomName
            }).ToListAsync();

            return bookings;
        }


        public async Task<List<ResponseCommunityRoomBookingDTO>> GetBookings()
        {
            return await _unitOfWork.GetRepository<CommunityRoomBooking>().Entities
                .Include(b => b.Apartment)
                .Include(b => b.CommunityRoom)
                .Select(booking => new ResponseCommunityRoomBookingDTO
                {
                    BookingId = booking.CommunityRoomBookingId,
                    ApartmentCode = booking.Apartment.ApartmentCode!,
                    BookingDate = booking.BookingDate,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    NumberOfPeople = booking.NumberOfPeople,
                    RoomName = booking.CommunityRoom.RoomName
                }).ToListAsync();
        }
        public async Task<bool> IsRoomAvailable(int communityRoomId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime, int numberOfPeople)
        {
            var existingBookings = await _unitOfWork.GetRepository<CommunityRoomBooking>()
                .Entities
                .Where(b => b.CommunityRoomId == communityRoomId
                    && b.BookingDate == bookingDate
                    && b.StartTime < endTime && b.EndTime > startTime)
                .ToListAsync();

            var totalPeopleBooked = existingBookings.Sum(b => b.NumberOfPeople);

            var room = await _unitOfWork.GetRepository<CommunityRoom>()
                .Entities
                .FirstOrDefaultAsync(r => r.CommunityRoomId == communityRoomId);

            if (room == null)
            {
                return false; 
            }

            return totalPeopleBooked + numberOfPeople <= room.RoomSize;
        }


        public async Task<bool> CreateBooking(int communityRoomId, int apartmentId, DateOnly bookingDate,
            TimeOnly startTime, TimeOnly endTime, int numberOfPeople)
        {
            try
            {
                bool isAvailable = await IsRoomAvailable(communityRoomId, bookingDate, startTime, endTime, numberOfPeople);

                if (!isAvailable)
                {
                    return false; 
                }

                var booking = new CommunityRoomBooking
                {
                    CommunityRoomId = communityRoomId,
                    ApartmentId = apartmentId,
                    BookingDate = bookingDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    NumberOfPeople = numberOfPeople
                };

                await _unitOfWork.GetRepository<CommunityRoomBooking>().InsertAsync(booking);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteBooking(int bookingId)
        {
            try
            {
                await _unitOfWork.GetRepository<CommunityRoomBooking>().DeleteAsync(bookingId);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
