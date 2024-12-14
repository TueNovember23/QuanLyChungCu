using Repositories.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AccountantServices
{
    public interface IFeeService
    {
            Task<List<WaterFee>> GetWaterFees();
            Task<List<ManagementFee>> GetManagementFees();
            Task<List<VehicleCategory>> GetVehicleCategories();

            Task AddWaterFee(WaterFee waterFee);
            Task AddManagementFee(ManagementFee managementFee);
            Task AddVehicleCategory(VehicleCategory vehicleCategory);
    }
}
