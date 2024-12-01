using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.CommunityRoomBookingDTO;
using Services.Interfaces.ServiceSupervisorServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<ResponseCommunityRoomDTO>> Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await GetAll();
            }

            var query = _unitOfWork.GetRepository<CommunityRoom>().Entities
                .Include(r => r.CommunityRoomBookings)
                .Where(room =>
                    room.RoomName.Contains(searchText) ||
                    room.Location!.Contains(searchText));

            return await query.Select(room => new ResponseCommunityRoomDTO
            {
                CommunityRoomId = room.CommunityRoomId,
                RoomName = room.RoomName,
                RoomSize = room.RoomSize,
                Location = room.Location,
                CurrentBookings = room.CommunityRoomBookings.Count
            }).ToListAsync();
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
        public async Task<bool> IsRoomAvailable(int communityRoomId, DateOnly bookingDate,
        TimeOnly startTime, TimeOnly endTime)
        {
            var existingBookings = await _unitOfWork.GetRepository<CommunityRoomBooking>()
                .Entities
                .Where(b => b.CommunityRoomId == communityRoomId
                    && b.BookingDate == bookingDate)
                .ToListAsync();

            // Check if there are any overlapping bookings
            foreach (var booking in existingBookings)
            {
                if (booking.StartTime < endTime && booking.EndTime > startTime)
                {
                    return false; // Room is not available - time slot overlap
                }
            }

            // Get room capacity
            var room = await _unitOfWork.GetRepository<CommunityRoom>()
                .Entities
                .FirstOrDefaultAsync(r => r.CommunityRoomId == communityRoomId);

            if (room == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CreateBooking(int communityRoomId, int apartmentId, DateOnly bookingDate,
            TimeOnly startTime, TimeOnly endTime, int numberOfPeople)
        {
            try
            {
                // First check if the room is available
                bool isAvailable = await IsRoomAvailable(communityRoomId, bookingDate, startTime, endTime);

                if (!isAvailable)
                {
                    return false; // Room is not available
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
