using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.Base;
using Repositories.Repositories.Entities;

namespace Repositories.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateVehicleAsync(Vehicle vehicle)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingVehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.VehicleId == vehicle.VehicleId);
                if (existingVehicle != null)
                    return false;

                await _context.Vehicles.AddAsync(vehicle);
                var result = await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
                return result > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<Vehicle> GetNumberAsync(string number)
        {
            return await _context.Vehicles
                .Include(v => v.VehicleCategory)
                .FirstOrDefaultAsync(v => v.VehicleId == number);
        }

        public async Task<float> GetFeeByTypeAsync(string vehicleType)
        {
            var category = await _context.VehicleCategories
                .FirstOrDefaultAsync(c => c.CategoryName == vehicleType);
            return (float)(category?.MonthlyFee ?? 0);
        }

        // ------

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles
                .Include(v => v.VehicleCategory)
                .Include(v => v.Apartment)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> SearchAsync(string searchText)
        {
            return await _context.Vehicles
                .Include(v => v.VehicleCategory)
                .Include(v => v.Apartment)
                .Where(v => v.VehicleId.Contains(searchText) ||
                           v.VehicleOwner.Contains(searchText))
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(string vehicleNumber)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.VehicleId == vehicleNumber);

                if (vehicle == null) return false;

                _context.Vehicles.Remove(vehicle);
                var result = await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return result > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<Vehicle> GetByNumberAsync(string vehicleNumber)
        {
            return await _context.Vehicles
                .Include(v => v.VehicleCategory)
                .Include(v => v.Apartment)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleNumber);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ------ Apartment

        public async Task<List<Apartment>> GetAllApartAsync()
        {
            return await _context.Apartments
                .Include(a => a.Floor)
                .Include(a => a.Representatives)
                .ToListAsync();
        }

        public async Task<List<Apartment>> SearchApartByCodeAsync(string searchText)
        {
            return await _context.Apartments
                .Include(a => a.Floor)
                .Include(a => a.Representatives)
                .Where(a => a.ApartmentCode.Contains(searchText))
                .ToListAsync();
        }

        public async Task<Apartment> GetApartByIdAsync(int id)
        {
            return await _context.Apartments
                .Include(a => a.Floor)
                .Include(a => a.Representatives)
                .FirstOrDefaultAsync(a => a.ApartmentId == id);
        }
    }
}
