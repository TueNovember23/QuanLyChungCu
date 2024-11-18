using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using Services.DTOs.ApartmentDTO;
using Services.Interfaces.AccountantServices;

namespace Services.Services.AccountantServices
{
    public class ApartmentService(IUnitOfWork unitOfWork) : IApartmentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<ResponseApartmentDTO>> GetAll()
        {
            List<ResponseApartmentDTO> apartments = await _unitOfWork.GetRepository<Apartment>().Entities
                .Select(Apartment => new ResponseApartmentDTO
                {
                    ApartmentCode = $"{Apartment.Floor.FloorNumber}.{Apartment.ApartmentId} Block {Apartment.Floor.Block.BlockCode}",
                    Area = Apartment.Area,
                    Block = Apartment.Floor.Block.BlockCode,
                    Floor = Apartment.Floor.FloorNumber,
                    Status = Apartment.Status
                }).ToListAsync();

            return apartments;
        }
    }
}
