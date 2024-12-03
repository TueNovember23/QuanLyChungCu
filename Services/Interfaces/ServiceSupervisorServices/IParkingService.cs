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
        // Add new methods
        Task<List<ParkingSpaceDTO>> GetParkingSpacesAsync();
        Task<VehicleLimitDTO> GetVehicleLimitsByApartmentAsync(int apartmentId);
        Task<bool> ValidateVehicleRegistrationAsync(string vehicleType, int apartmentId);
        Task<(List<ParkingSpaceDTO> spaces, VehicleLimitDTO limits)> GetParkingDataAsync(int apartmentId, CancellationToken cancellationToken = default);
    }
}