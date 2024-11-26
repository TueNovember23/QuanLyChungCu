using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.VehicleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.ServiceSupervisorServices
{
    public interface IVehicleService
    {
        public Task RegisterVehicleAsync(VehicleDTO vehicle);
        public Task<List<VehicleResponseDTO>> GetAllVehiclesAsync();
        public Task<VehicleResponseDTO> GetVehicleByNumberAsync(string vehicleNumber);
        public Task<List<ApartmentDTO>> SearchApartmentsAsync(string searchText);
        public Task<List<ApartmentDTO>> GetAllApartmentsAsync();
        public float CalculatePaymentAmount(string vehicleType);
        public Task<List<VehicleResponseDTO>> GetParkingAsync(string searchText);
        public Task<List<VehicleResponseDTO>> SearchVehiclesAsync(string searchText);
        public Task DeleteVehicleAsync(string vehicleNumber);
    }
}
