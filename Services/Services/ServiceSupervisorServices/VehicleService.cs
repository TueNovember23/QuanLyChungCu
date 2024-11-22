using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Services.DTOs.VehicleDTO;
using Services.DTOs.ApartmentDTO;
using Services.Interfaces.ServiceSupervisorServices;
using Core;

namespace Services.Services.ServiceSupervisorServices
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }



        public async Task RegisterVehicleAsync(VehicleDTO vehicle)
        {
            if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));

            // Check for existing vehicle
            var existingVehicle = await _unitOfWork.GetRepository<Vehicle>()
                .FindAsync(v => v.VehicleId == vehicle.VehicleNumber);

            if (existingVehicle != null)
            {
                throw new BusinessException("Biển số xe này đã được đăng ký!");
            }

            var entity = new Vehicle
            {
                VehicleId = vehicle.VehicleNumber ?? string.Empty,
                VehicleCategoryId = GetCategoryId(vehicle.VehicleType ?? string.Empty),
                VehicleOwner = vehicle.VehicleOwner ?? string.Empty, // Fixed line
                ApartmentId = vehicle.ApartmentId
            };

            _unitOfWork.GetRepository<Vehicle>().Insert(entity);
            await _unitOfWork.SaveAsync();
        }


        public async Task<List<VehicleResponseDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _unitOfWork.GetRepository<Vehicle>().GetAllAsync();
            return vehicles.Select(v => new VehicleResponseDTO
            {
                VehicleNumber = v.VehicleId,
                VehicleType = v.VehicleCategory?.CategoryName ?? string.Empty,
                VehicleOwner = v.VehicleOwner ?? string.Empty, // Fixed line
                MonthlyFee = (float)(v.VehicleCategory?.MonthlyFee ?? 0),
                Status = "Active" // Assuming a default status
            }).ToList();
        }

        public async Task<VehicleResponseDTO> GetVehicleByNumberAsync(string vehicleNumber)
        {
            if (string.IsNullOrEmpty(vehicleNumber))
                throw new ArgumentNullException(nameof(vehicleNumber));

            var vehicle = await _unitOfWork.GetRepository<Vehicle>()
                .FindAsync(v => v.VehicleId == vehicleNumber);

            if (vehicle == null)
                return new VehicleResponseDTO(); 

            return new VehicleResponseDTO
            {
                VehicleNumber = vehicle.VehicleId,
                VehicleType = vehicle.VehicleCategory?.CategoryName ?? string.Empty,
                VehicleOwner = vehicle.VehicleOwner ?? string.Empty, // Fixed line
                MonthlyFee = (float)(vehicle.VehicleCategory?.MonthlyFee ?? 0),
                Status = "Active"
            };
        }

        public async Task<List<ApartmentDTO>> SearchApartmentsAsync(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return new List<ApartmentDTO>();

            var apartments = await _unitOfWork.GetRepository<Apartment>()
                .FindAllAsync(a => a.ApartmentCode != null && a.ApartmentCode.Contains(searchText));

            return apartments.Select(a => new ApartmentDTO
            {
                ApartmentId = a.ApartmentId,
                ApartmentCode = a.ApartmentCode ?? string.Empty,
                Status = a.Status ?? string.Empty,
                FloorNumber = a.Floor.FloorNumber
            }).ToList();
        }

        public async Task<List<ApartmentDTO>> GetAllApartmentsAsync()
        {
            var apartments = await _unitOfWork.GetRepository<Apartment>()
                .FindListAsync(a => !a.IsDeleted);

            return apartments
                .Where(a => a.ApartmentCode != null)
                .Select(a => new ApartmentDTO
                {
                    ApartmentId = a.ApartmentId,
                    ApartmentCode = a.ApartmentCode ?? string.Empty,
                    Status = a.Status ?? string.Empty,
                    FloorNumber = a.Floor?.FloorNumber ?? 0,
                    OwnerName = a.Representatives
                                .FirstOrDefault(r => r.ApartmentId == a.ApartmentId)?.FullName ?? "Chưa có chủ hộ"
                }).ToList();
        }
        

        // public async Task<List<ApartmentDTO>> GetAllApartmentsAsync()
        // {
        //     var apartments = await _unitOfWork.GetRepository<Apartment>()
        //         .FindListAsync(a => !a.IsDeleted);

        //     return apartments
        //         .Where(a => a.ApartmentCode != null)
        //         .Select(a => new ApartmentDTO
        //         {
        //             ApartmentId = a.ApartmentId,
        //             ApartmentCode = a.ApartmentCode ?? "",
        //             Status = a.Status ?? "Unknown",
        //             FloorNumber = a.Floor?.FloorNumber ?? 0
        //         }).ToList();
        // }


        public float CalculatePaymentAmount(string vehicleType)
        {
            return vehicleType switch
            {
                "Xe đạp" => 50000,
                "Xe máy" => 100000,
                "Ô tô" => 200000,
                "Xe máy điện" => 120000,
                "Ô tô điện" => 250000,
                _ => throw new ArgumentException("Invalid vehicle type")
            };
        }

        private int GetCategoryId(string vehicleType)
        {
            // Map vehicle type to category ID (simplified logic)
            return vehicleType switch
            {
                "Xe đạp" => 1,
                "Xe máy" => 2,
                "Ô tô" => 3,
                "Xe máy điện" => 4,
                "Ô tô điện" => 5,
                _ => throw new ArgumentException("Invalid vehicle type")
            };
        }
    }
}