using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.Entities;
using Repositories.Interfaces;
using Services.DTOs.ApartmentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using AutoMapper;
using Core;

namespace Services.Services.AdministrativeStaffServices
{
    public class ApartmentService(IUnitOfWork unitOfWork) : IApartmentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        //public ApartmentService(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}

        public async Task<List<ResponseApartmentDTO>> GetAll()
        {
            List<ResponseApartmentDTO> apartments = await _unitOfWork.GetRepository<Apartment>().Entities
                .Select(Apartment => new ResponseApartmentDTO
                {
                    ApartmentCode = Apartment.ApartmentCode!,
                    Area = Apartment.Area,
                    NumberOfPeople = Apartment.NumberOfPeople,
                    Block = Apartment.Floor.Block.BlockCode,
                    Floor = Apartment.Floor.FloorNumber,
                    Status = Apartment.Status
                }).ToListAsync();

            return apartments;
        }

        public async Task<List<ResponseApartmentDTO>> Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await GetAll();
            }

            var query = _unitOfWork.GetRepository<Apartment>().Entities
                .Include(apartment => apartment.Floor.Block)
                .Include(apartment => apartment.Representatives)
                .Where(apartment =>
                    (apartment.ApartmentCode != null && apartment.ApartmentCode.Contains(searchText)) ||
                    (apartment.Floor.Block.BlockCode != null && apartment.Floor.Block.BlockCode.Contains(searchText)) ||
                    apartment.Floor.FloorNumber.ToString().Contains(searchText) ||
                    (apartment.Status != null && apartment.Status.Contains(searchText)) ||
                    apartment.Representatives.Any(r => r.FullName != null && r.FullName.Contains(searchText))
                );

            var response = await query.Select(apartment => new ResponseApartmentDTO
            {
                ApartmentCode = apartment.ApartmentCode!,
                Area = apartment.Area,
                NumberOfPeople = apartment.NumberOfPeople,
                Block = apartment.Floor.Block.BlockCode,
                Floor = apartment.Floor.FloorNumber,
                Status = apartment.Status
            }).ToListAsync();

            return response;
        }

        public async Task<Apartment?> GetApartmentByCode(string code)
        {
            return await _unitOfWork.GetRepository<Apartment>().Entities
                .Include(apartment => apartment.Floor.Block)
                .Include(apartment => apartment.Representatives)
                .FirstOrDefaultAsync(apartment => apartment.ApartmentCode == code);
        }

        public async Task<List<ResponseApartmentDTO>> GetAllApartmentForViolation()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Apartment>();
                // Use ToListAsync() first to materialize the data before projecting
                var apartments = await repository.Entities
                    .AsNoTracking()
                    .Include(a => a.Floor)
                        .ThenInclude(f => f.Block)
                    .ToListAsync();

                // Then perform the projection on the materialized data
                var result = apartments.Select(apartment => new ResponseApartmentDTO
                {
                    ApartmentId = apartment.ApartmentId,
                    ApartmentCode = apartment.ApartmentCode!,
                    Area = apartment.Area,
                    NumberOfPeople = apartment.NumberOfPeople,
                    Block = apartment.Floor.Block.BlockCode,
                    Floor = apartment.Floor.FloorNumber,
                    Status = apartment.Status
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error getting apartments: {ex.Message}");
            }
        }
    }
}
