using Repositories.Repositories.Entities;

namespace Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        Task<bool> CreateVehicleAsync(Vehicle vehicle);
        Task<Vehicle> GetNumberAsync(string number);
        Task<float> GetFeeByTypeAsync(string vehicleType);

        // --------------- Parking

        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<IEnumerable<Vehicle>> SearchAsync(string searchText);
        Task<bool> DeleteAsync(string vehicleNumber);
        Task<Vehicle> GetByNumberAsync(string vehicleNumber);
        Task<int> SaveChangesAsync();

        // --------------- Apartment
        Task<List<Apartment>> GetAllApartAsync();
        Task<List<Apartment>> SearchApartByCodeAsync(string searchText);
        Task<Apartment> GetApartByIdAsync(int id);

        // ---------------  
        Task<int> GetApartmentCountAsync();
        Task<List<ParkingConfig>> GetParkingConfigAsync();
        Task<List<Vehicle>> GetVehiclesByApartmentAsync(int apartmentId);

        Task<(
            int totalSpaces,
            List<ParkingConfig> spaces,
            Dictionary<int, int> usedSpaces,
            List<Vehicle> apartmentVehicles
        )> GetParkingDataAsync(int apartmentId, CancellationToken cancellationToken = default);
    }
}
