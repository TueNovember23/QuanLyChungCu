using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountantServices
{
    public class FeeService : IFeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<WaterFee>> GetWaterFees()
        {
            var repository = _unitOfWork.GetRepository<WaterFee>();
            var result = await repository.FindListAsync(w => w.DeletedDate == null);
            return result.OrderByDescending(w => w.CreatedDate).ToList();
        }

        public async Task<List<ManagementFee>> GetManagementFees()
        {
            var repository = _unitOfWork.GetRepository<ManagementFee>();
            var result = await repository.FindListAsync(m => m.DeletedDate == null);
            return result.OrderByDescending(m => m.CreatedDate).ToList();
        }

        public async Task<List<VehicleCategory>> GetVehicleCategories()
        {
            var repository = _unitOfWork.GetRepository<VehicleCategory>();
            var result = await repository.GetAllAsync();
            return result.OrderBy(v => v.CategoryName).ToList();
        }

        public async Task AddWaterFee(WaterFee waterFee)
        {
            var repository = _unitOfWork.GetRepository<WaterFee>();

            // Đánh dấu xóa mềm các bản ghi cũ
            var oldFees = await repository.FindListAsync(w => w.DeletedDate == null);
            foreach (var oldFee in oldFees)
            {
                oldFee.DeletedDate = DateOnly.FromDateTime(DateTime.Now);
                repository.Update(oldFee);
            }

            // Thêm bản ghi mới
            await repository.InsertAsync(waterFee);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddManagementFee(ManagementFee managementFee)
        {
            var repository = _unitOfWork.GetRepository<ManagementFee>();

            // Đánh dấu xóa mềm các bản ghi cũ
            var oldFees = await repository.FindListAsync(m => m.DeletedDate == null);
            foreach (var oldFee in oldFees)
            {
                oldFee.DeletedDate = DateOnly.FromDateTime(DateTime.Now);
                repository.Update(oldFee);
            }

            // Thêm bản ghi mới
            await repository.InsertAsync(managementFee);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddVehicleCategory(VehicleCategory vehicleCategory)
        {
            var repository = _unitOfWork.GetRepository<VehicleCategory>();

            // Tìm loại xe cùng tên nếu có
            var existingCategory = (await repository.FindListAsync(v => v.CategoryName == vehicleCategory.CategoryName))
                                 .FirstOrDefault();

            if (existingCategory != null)
            {
                // Cập nhật giá mới cho loại xe đã tồn tại
                existingCategory.MonthlyFee = vehicleCategory.MonthlyFee;
                repository.Update(existingCategory);
            }
            else
            {
                // Thêm loại xe mới nếu chưa tồn tại
                await repository.InsertAsync(vehicleCategory);
            }

            await _unitOfWork.SaveAsync();
        }
    }
}
