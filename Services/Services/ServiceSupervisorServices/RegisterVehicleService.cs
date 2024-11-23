using AutoMapper;
using Core;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.VehicleDTO;
using Services.Interfaces.ServiceSupervisorServices;

namespace Services.Services.ServiceSupervisorServices
{
    public class RegisterVehicleService : IRegisterVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public RegisterVehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<List<ApartmentDTO>> GetAllApartmentsAsync()
        {
            var apartments = await _vehicleRepository.GetAllApartAsync();
            return apartments.Select(MapToApartmentDTO).ToList();
        }

        public async Task<List<ApartmentDTO>> SearchApartmentsAsync(string searchText)
        {
            var apartments = await _vehicleRepository.SearchApartByCodeAsync(searchText);
            return apartments.Select(MapToApartmentDTO).ToList();
        }

        public async Task<VehicleResponseDTO?> GetVehicleByNumberAsync(string number)
        {
            var vehicle = await _vehicleRepository.GetByNumberAsync(number);
            if (vehicle == null) return null;

            return new VehicleResponseDTO
            {
                Success = true,
                VehicleNumber = vehicle.VehicleId,
                VehicleType = vehicle.VehicleCategory?.CategoryName ?? "Unknown",
                VehicleOwner = vehicle.VehicleOwner,
                PaymentAmount = (float)(vehicle.VehicleCategory?.MonthlyFee ?? 0)
            };
        }

        public async Task<VehicleResponseDTO> RegisterVehicleAsync(VehicleDTO vehicleDto)
        {

            if (!vehicleDto.IsValid())
            {
                return new VehicleResponseDTO 
                { 
                    Success = false,
                    Message = "Thông tin xe không hợp lệ"
                };
            }

            var vehicleCount = await _vehicleRepository.GetAllAsync();
            var apartmentVehicles = vehicleCount.Count(v => v.ApartmentId == vehicleDto.ApartmentId);
            if (apartmentVehicles >= 3)
            {
                return new VehicleResponseDTO
                {
                    Success = false,
                    Message = "Căn hộ đã đăng ký tối đa 3 xe"
                };
            }
            var response = new VehicleResponseDTO();

            try
            {
                var existingVehicle = await _vehicleRepository.GetByNumberAsync(vehicleDto.VehicleNumber);
                if (existingVehicle != null)
                {
                    response.Success = false;
                    response.Message = "Biển số xe đã tồn tại";
                    return response;
                }

                var vehicle = new Vehicle
                {
                    VehicleId = vehicleDto.VehicleNumber,
                    VehicleCategoryId = GetCategoryId(vehicleDto.VehicleType),
                    VehicleOwner = vehicleDto.VehicleOwner,
                    ApartmentId = vehicleDto.ApartmentId
                };

                var success = await _vehicleRepository.CreateVehicleAsync(vehicle);
                if (success)
                {
                    await _vehicleRepository.SaveChangesAsync();
                    response.Success = true;
                    response.Message = "Đăng ký xe thành công";
                    response.PaymentAmount = CalculatePaymentAmount(vehicleDto.VehicleType);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Không thể đăng ký xe";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi: {ex.Message}";
            }

            return response;
        }

        public float CalculatePaymentAmount(string vehicleType)
        {
            return vehicleType switch
            {
                "Xe đạp" => 50000f,
                "Xe máy" => 100000f,
                "Ô tô" => 1200000f,
                "Xe máy điện" => 80000f,
                "Ô tô điện" => 1000000f,
                _ => 0f
            };
        }


        private ApartmentDTO MapToApartmentDTO(Apartment apartment)
        {
            return new ApartmentDTO
            {
                ApartmentId = apartment.ApartmentId,
                ApartmentCode = apartment.ApartmentCode,
                Status = apartment.Status,
                FloorNumber = apartment.Floor?.FloorNumber ?? 0,
                OwnerName = apartment.Representatives
                            .FirstOrDefault()?.FullName ?? "Chưa có chủ hộ"
            };
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
    }
}