using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.AccountantServices
{
    public class FeeService : IFeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly object _lock = new object();

        public FeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<WaterFee>> GetWaterFees()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<WaterFee>();
                var result = await repository.FindListAsync(w => w.DeletedDate == null);
                return result.OrderByDescending(w => w.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting water fees: {ex.Message}");
            }
        }

        public async Task<List<ManagementFee>> GetManagementFees()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<ManagementFee>();
                var result = await repository.FindListAsync(m => m.DeletedDate == null);
                return result.OrderByDescending(m => m.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting management fees: {ex.Message}");
            }
        }

        public async Task<List<VehicleCategory>> GetVehicleCategories()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<VehicleCategory>();
                var result = await repository.GetAllAsync();
                return result.OrderBy(v => v.CategoryName).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting vehicle categories: {ex.Message}");
            }
        }

        public async Task AddWaterFee(WaterFee waterFee)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error adding water fee: {ex.Message}");
            }
        }

        public async Task AddManagementFee(ManagementFee managementFee)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error adding management fee: {ex.Message}");
            }
        }

        public async Task AddVehicleCategory(VehicleCategory vehicleCategory)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<VehicleCategory>();

                // Tìm loại xe cùng tên nếu có
                var existingCategories = await repository.FindListAsync(v => v.CategoryName == vehicleCategory.CategoryName);
                var existingCategory = existingCategories.FirstOrDefault();

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
            catch (Exception ex)
            {
                throw new Exception($"Error adding/updating vehicle category: {ex.Message}");
            }
        }
    }
}