using Services.DTOs.VehicleDTO;

namespace Services.Interfaces.ServiceSupervisorServices
{
    public interface IParkingService
    {
        Task<List<VehicleResponseDTO>> GetAllVehiclesAsync();
        Task<List<VehicleResponseDTO>> SearchVehiclesAsync(string searchText);
        Task<bool> DeleteVehicleAsync(string vehicleNumber);
        Task<VehicleStatisticsDTO> GetVehicleStatisticsAsync();
        Task<bool> UpdateVehicleAsync(VehicleDTO updatedVehicle);
    }
}