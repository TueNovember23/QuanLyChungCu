using Services.DTOs.VehicleDTO;
using Services.DTOs.ApartmentDTO;

namespace Services.Interfaces.ServiceSupervisorServices
{
    public interface IVehicleService
    {
        Task RegisterVehicleAsync(VehicleDTO vehicle);
        Task<List<VehicleResponseDTO>> GetAllVehiclesAsync();
        Task<VehicleResponseDTO> GetVehicleByNumberAsync(string vehicleNumber);
        Task<List<ApartmentDTO>> SearchApartmentsAsync(string searchText);
        Task<List<ApartmentDTO>> GetAllApartmentsAsync();
        float CalculatePaymentAmount(string vehicleType);
        Task<List<VehicleResponseDTO>> GetParkingAsync(string searchText);
        Task<List<VehicleResponseDTO>> SearchVehiclesAsync(string searchText);
        Task DeleteVehicleAsync(string vehicleNumber);
    }

}