using Services.DTOs.VehicleDTO;
using Services.DTOs.ApartmentDTO;

namespace Services.Interfaces.ServiceSupervisorServices
{
    public interface IRegisterVehicleService
    {
        Task<List<ApartmentDTO>> GetAllApartmentsAsync();
        Task<List<ApartmentDTO>> SearchApartmentsAsync(string searchText);
        Task<VehicleResponseDTO> RegisterVehicleAsync(VehicleDTO request);
        Task<VehicleResponseDTO?> GetVehicleByNumberAsync(string vehicleNumber);
        float CalculatePaymentAmount(string vehicleType);
    }
}