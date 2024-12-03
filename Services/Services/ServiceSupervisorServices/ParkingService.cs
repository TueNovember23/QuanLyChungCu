using AutoMapper;
using Core;
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
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return await GetAllVehiclesAsync();
            }

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
                BicycleCount = vehicles.Count(v => v.VehicleCategory?.CategoryName == "Xe đạp"),
                MotorcycleCount = vehicles.Count(v => v.VehicleCategory?.CategoryName == "Xe máy"),
                CarCount = vehicles.Count(v => v.VehicleCategory?.CategoryName == "Ô tô"),
                ElectricMotorcycleCount = vehicles.Count(v => v.VehicleCategory?.CategoryName == "Xe máy điện"),
                ElectricCarCount = vehicles.Count(v => v.VehicleCategory?.CategoryName == "Ô tô điện")
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
                Status = vehicle.Status ?? "Đang gửi", // Default to "Đang gửi"
                MonthlyFee = (float)(vehicle.VehicleCategory?.MonthlyFee ?? 0),
                ApartmentId = vehicle.ApartmentId
            };
        }

        public async Task<bool> UpdateVehicleAsync(VehicleDTO updatedVehicle)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByNumberAsync(updatedVehicle.VehicleNumber);
                if (vehicle == null) return false;

                vehicle.VehicleOwner = updatedVehicle.VehicleOwner;
                vehicle.VehicleCategoryId = GetCategoryId(updatedVehicle.VehicleType);
                vehicle.Status = updatedVehicle.Status; // Update status

                var result = await _vehicleRepository.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        private int GetCategoryId(string vehicleType)
        {
            return vehicleType switch
            {
                "Xe đạp" => 1,
                "Xe máy" => 2,
                "Ô tô" => 3,
                "Xe máy điện" => 4,
                "Ô tô điện" => 5,
                _ => throw new ArgumentException("Loại xe không hợp lệ!")
            };
        }

        // Add new implementations
        public async Task<List<ParkingSpaceDTO>> GetParkingSpacesAsync()
        {
            var totalApartments = await _vehicleRepository.GetApartmentCountAsync();
            var config = await _vehicleRepository.GetParkingConfigAsync();
            var vehicles = await _vehicleRepository.GetAllAsync();

            return config.Select(c => new ParkingSpaceDTO
            {
                VehicleType = c.Category.CategoryName,
                MaxPerApartment = c.MaxPerApartment,
                TotalSpaces = (int)(totalApartments * c.TotalSpacePercent / 100.0),
                UsedSpaces = vehicles.Count(v => v.VehicleCategoryId == c.CategoryId)
            }).ToList();
        }

        public async Task<VehicleLimitDTO> GetVehicleLimitsByApartmentAsync(int apartmentId)
        {
            var vehicles = await _vehicleRepository.GetVehiclesByApartmentAsync(apartmentId);
            
            return new VehicleLimitDTO
            {
                CurrentBicycles = vehicles.Count(v => v.VehicleCategory?.CategoryName == "Xe đạp"),
                CurrentMotorcycles = vehicles.Count(v => 
                    v.VehicleCategory?.CategoryName == "Xe máy" || 
                    v.VehicleCategory?.CategoryName == "Xe máy điện"),
                CurrentCars = vehicles.Count(v => 
                    v.VehicleCategory?.CategoryName == "Ô tô" ||
                    v.VehicleCategory?.CategoryName == "Ô tô điện")
            };
        }

        public async Task<bool> ValidateVehicleRegistrationAsync(string vehicleType, int apartmentId)
        {
            var limits = await GetVehicleLimitsByApartmentAsync(apartmentId);
            var spaces = await GetParkingSpacesAsync();

            var spaceInfo = spaces.FirstOrDefault(s => s.VehicleType == vehicleType);
            if (spaceInfo == null || spaceInfo.AvailableSpaces <= 0)
                return false;

            return vehicleType switch
            {
                "Xe đạp" => limits.CurrentBicycles < limits.MaxBicycles,
                "Xe máy" or "Xe máy điện" => limits.CurrentMotorcycles < limits.MaxMotorcycles,
                "Ô tô" or "Ô tô điện" => limits.CurrentCars < limits.MaxCars,
                _ => false
            };
        }

        public async Task<(List<ParkingSpaceDTO> spaces, VehicleLimitDTO limits)> GetParkingDataAsync(
            int apartmentId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Lấy spaces và limits trong một lần gọi để tránh race condition
                var spaces = await GetParkingSpacesAsync();
                var limits = await GetVehicleLimitsByApartmentAsync(apartmentId);

                return (spaces, limits);
            }
            catch (Exception ex) 
            {
                throw new BusinessException($"Lỗi khi lấy thông tin bãi đỗ xe: {ex.Message}");
            }
        }
    }
}