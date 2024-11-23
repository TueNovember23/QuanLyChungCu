using AutoMapper;
using Repositories.Repositories.Entities;
using Repositories.Interfaces;
using Services.DTOs.VehicleDTO;
using Services.DTOs.ApartmentDTO;
using Services.Interfaces.ServiceSupervisorServices;
using Core;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
                VehicleOwner = vehicle.VehicleOwner ?? string.Empty, 
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
                VehicleOwner = v.VehicleOwner ?? string.Empty, 
                MonthlyFee = (float)(v.VehicleCategory?.MonthlyFee ?? 0),
                Status = "Active" 
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
                VehicleOwner = vehicle.VehicleOwner ?? string.Empty,
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
        

        public float CalculatePaymentAmount(string vehicleType)
        {
            return vehicleType switch
            {
                "Xe đạp" => 50000,
                "Xe máy" => 100000,
                "Ô tô" => 200000,
                "Xe máy điện" => 120000,
                "Ô tô điện" => 250000,
                _ => throw new ArgumentException("Loại xe không hợp lệ!")
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


        // -------------------- Parking View
        public async Task<List<VehicleResponseDTO>> GetParkingAsync(string searchText)
        {
            try
            {
                var vehicleRepository = _unitOfWork.GetRepository<Vehicle>();
                IQueryable<Vehicle> vehicles;

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    vehicles = vehicleRepository.FindAll(); 
                }
                else
                {
                    vehicles = vehicleRepository.Find_Sync(v => v.VehicleId.Contains(searchText) || v.VehicleOwner.Contains(searchText));
                }

                var vehicleList = await vehicles
                    .Include(v => v.VehicleCategory)
                    .Include(v => v.Apartment)
                    .ToListAsync();

                return vehicleList.Select(v => new VehicleResponseDTO
                {
                    VehicleNumber = v.VehicleId,
                    VehicleType = v.VehicleCategory?.CategoryName ?? string.Empty,
                    VehicleOwner = v.VehicleOwner ?? string.Empty,
                    ApartmentCode = v.Apartment?.ApartmentCode ?? string.Empty,
                    Status = "Đã thanh toán",
                    MonthlyFee = (float)(v.VehicleCategory?.MonthlyFee ?? 0)
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi từ GetParkingAsync: {ex.Message}");
                return new List<VehicleResponseDTO>();
            }
        }

        public async Task<List<VehicleResponseDTO>> SearchVehiclesAsync(string searchText)
        {
            var vehicleRepository = _unitOfWork.GetRepository<Vehicle>();
            var vehicles = await vehicleRepository
                .FindAllAsync(v => v.VehicleId.Contains(searchText) || v.VehicleOwner.Contains(searchText));

            var vehicleList = await vehicles
                .Include(v => v.VehicleCategory)
                .Include(v => v.Apartment)
                .ToListAsync();

            return vehicleList.Select(v => new VehicleResponseDTO
            {
                VehicleNumber = v.VehicleId,
                VehicleType = v.VehicleCategory?.CategoryName ?? string.Empty,
                VehicleOwner = v.VehicleOwner ?? string.Empty,
                ApartmentCode = v.Apartment?.ApartmentCode ?? string.Empty,
                Status = "Đã thanh toán", 
                MonthlyFee = (float)(v.VehicleCategory?.MonthlyFee ?? 0)
            }).ToList();
        }

        public async Task DeleteVehicleAsync(string vehicleNumber)
        {
            var vehicle = await _unitOfWork.GetRepository<Vehicle>().FindAsync(v => v.VehicleId == vehicleNumber);
            if (vehicle != null)
            {
                _unitOfWork.GetRepository<Vehicle>().Delete(vehicle);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}