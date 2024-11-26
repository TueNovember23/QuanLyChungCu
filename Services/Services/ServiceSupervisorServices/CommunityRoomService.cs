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

        public Task<bool> CreateBookingAsync(CommunityRoomBooking booking)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBookingAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GenerateBookingReceiptAsync(int bookingId)
        {
            throw new NotImplementedException();
        }

        public async Task<ObservableCollection<CommunityRoomBookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork
                .GetRepository<CommunityRoomBooking>()
                .Entities
                .Include(b => b.Apartment)
                .ToListAsync();

            // Chuyển đổi dữ liệu sang DTO
            var bookingDTOs = bookings.Select(b => new CommunityRoomBookingDTO
            {
                BookingId = b.CommunityRoomBookingId,
                BookingDate = b.BookingDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                NumberOfPeople = b.NumberOfPeople,
                ApartmentCode = b.Apartment.ApartmentCode
            });

            return new ObservableCollection<CommunityRoomBookingDTO>(bookingDTOs);
        }

        public async Task RegisterCommunityRoomAsync(CommunityRoomBookingDTO bookingDTO)
        {
            var booking = new CommunityRoomBooking
            {
                CommunityRoomBookingId = 0, // Để Entity Framework tự sinh ID
                BookingDate = bookingDTO.BookingDate,
                StartTime = bookingDTO.StartTime,
                EndTime = bookingDTO.EndTime,
                NumberOfPeople = bookingDTO.NumberOfPeople,
                ApartmentId = (await _unitOfWork
                    .GetRepository<Apartment>()
                    .Entities
                    .FirstOrDefaultAsync(a => a.ApartmentCode == bookingDTO.ApartmentCode))?.ApartmentId ?? throw new Exception("Apartment not found")
            };

            await _unitOfWork.GetRepository<CommunityRoomBooking>().InsertAsync(booking);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<CommunityRoomBookingDTO>> SearchBookingsAsync(string? apartmentCode, DateTime? bookingDate)
        {
            var query = _unitOfWork.GetRepository<CommunityRoomBooking>().Entities.Include(b => b.Apartment).AsQueryable();

            if (!string.IsNullOrEmpty(apartmentCode))
            {
                query = query.Where(b => b.Apartment.ApartmentCode == apartmentCode);
            }

            if (bookingDate.HasValue)
            {
                query = query.Where(b => b.BookingDate == DateOnly.FromDateTime(bookingDate.Value));

            }

            var results = await query.ToListAsync();

            return results.Select(b => new CommunityRoomBookingDTO
            {
                BookingId = b.CommunityRoomBookingId,
                BookingDate = b.BookingDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                NumberOfPeople = b.NumberOfPeople,
                ApartmentCode = b.Apartment.ApartmentCode
            });
        }

        public async Task<IEnumerable<ApartmentDTO>> GetAllApartmentsAsync()
        {
            var apartments = await _unitOfWork
                .GetRepository<Apartment>()
                .Entities
                .ToListAsync();

            return apartments.Select(a => new ApartmentDTO
            {
                ApartmentId = a.ApartmentId,
                ApartmentCode = a.ApartmentCode,
            });
        }


        public Task<bool> UpdateBookingAsync(CommunityRoomBooking booking)
        {
            throw new NotImplementedException();
        }
    }
}
