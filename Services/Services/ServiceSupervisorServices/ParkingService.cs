using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.VehicleDTO;
using Services.Interfaces.ServiceSupervisorServices;

namespace Services.Services.ServiceSupervisorServices
{
    public class ParkingService : IParkingService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public ParkingService(IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<List<VehicleResponseDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(MapToDTO).ToList();
        }

        public async Task<List<VehicleResponseDTO>> SearchVehiclesAsync(string searchText)
        {
            var vehicles = await _vehicleRepository.SearchAsync(searchText);
            return vehicles.Select(MapToDTO).ToList();
        }

        public async Task<bool> DeleteVehicleAsync(string vehicleNumber)
        {
            var success = await _vehicleRepository.DeleteAsync(vehicleNumber);
            if (success)
            {
                await _vehicleRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<VehicleStatisticsDTO> GetVehicleStatisticsAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return new VehicleStatisticsDTO
            {
                BicycleCount = vehicles.Count(v => v.VehicleCategory?.CategoryName?.Contains("Xe đạp") == true),
                MotorcycleCount = vehicles.Count(v => v.VehicleCategory?.CategoryName?.Contains("Xe máy") == true),
                CarCount = vehicles.Count(v => v.VehicleCategory?.CategoryName?.Contains("Ô tô") == true)
            };
        }

        private VehicleResponseDTO MapToDTO(Vehicle vehicle)
        {
            return new VehicleResponseDTO
            {
                VehicleNumber = vehicle.VehicleId ?? "Chưa có biển số",
                VehicleType = vehicle.VehicleCategory?.CategoryName ?? "Chưa xác định",
                VehicleOwner = vehicle.VehicleOwner ?? "Chưa có chủ xe",
                ApartmentCode = vehicle.Apartment?.ApartmentCode ?? "Chưa có mã căn hộ",
                Status = "Đã thanh toán",
                MonthlyFee = (float)(vehicle.VehicleCategory?.MonthlyFee ?? 0)
            };
        }
    }
}