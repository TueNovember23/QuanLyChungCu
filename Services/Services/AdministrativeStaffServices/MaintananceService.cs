using Core;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.MaintenanceDTO;
using Services.Interfaces.AdministrativeStaffServices;

namespace Services.Services.AdministrativeStaffServices
{
    public class MaintananceService : IMaintananceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MaintananceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseMaintenance>> GetAll()
        {
            List<ResponseMaintenance> maintenances = await _unitOfWork.GetRepository<Maintenance>().Entities
                .Select(_ => new ResponseMaintenance
                {
                    MaintenanceId = _.MaintenanceId,
                    MaintenanceName = _.MaintenanceName,
                    MaintanaceDate = _.MaintanaceDate.ToString("dd-MM-yyyy"), // Định dạng ngày
                    Description = _.Description,
                    Status = _.Status,
                    CreatedBy = _.CreatedByNavigation.FullName,
                    Department = _.Department.DepartmentName
                })
                .ToListAsync();
            return maintenances;
        }

        public async Task<List<ResponseMaintenance>> Search(string searchText)
        {
            int? maintenanceId = null;

            // Thử chuyển đổi searchText sang kiểu int (nếu là số)
            if (int.TryParse(searchText, out int id))
            {
                maintenanceId = id;
            }

            // Lọc dựa trên các tiêu chí
            List<ResponseMaintenance> maintenances = await _unitOfWork.GetRepository<Maintenance>().Entities
                .Where(_ =>
                    (maintenanceId != null && _.MaintenanceId == maintenanceId) || // Lọc theo ID
                    _.MaintenanceName.Contains(searchText) || // Lọc theo tên bảo trì
                    _.Equipment.Any(e => e.EquipmentName.Contains(searchText)) || // Lọc theo thiết bị
                    _.MaintanaceDate.ToString().Contains(searchText) || // Lọc theo ngày bảo trì
                    _.Department.DepartmentName.Contains(searchText) || // Lọc theo bộ phận
                    (_.Status != null && _.Status.Contains(searchText)) // Lọc theo trạng thái
                )
                .Select(_ => new ResponseMaintenance
                {
                    MaintenanceId = _.MaintenanceId,
                    MaintenanceName = _.MaintenanceName,
                    MaintanaceDate = _.MaintanaceDate.ToString("dd-MM-yyyy"), // Định dạng ngày
                    Description = _.Description,
                    Status = _.Status,
                    CreatedBy = _.CreatedByNavigation.FullName,
                    Department = _.Department.DepartmentName
                })
                .ToListAsync();

            return maintenances;
        }


        public async Task<Maintenance> GetMaintenanceById(int id)
        {
            return await _unitOfWork.GetRepository<Maintenance>().Entities.FirstOrDefaultAsync(_ => _.MaintenanceId == id)
                ?? throw new BusinessException($"Không tìm thấy lịch bảo trì có mã {id}");
        }

        public async Task CompleteMaintenance(int id)
        {
            Maintenance maintenance = await _unitOfWork.GetRepository<Maintenance>().Entities.FirstOrDefaultAsync(_ => _.MaintenanceId == id)
                ?? throw new BusinessException($"Không tìm thấy lịch bảo trì có mã {id}");
            maintenance.Status = "Đã hoàn thành";
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<ResponseEquipment>> GetAllEquipment()
        {
            List<ResponseEquipment> responses = await _unitOfWork.GetRepository<Equipment>().Entities
                .Select(_ => new ResponseEquipment
                {
                    EquipmentId = _.EquipmentId,
                    EquipmentName = _.EquipmentName
                })
                .ToListAsync();
            return responses;
        }

        public async Task<int> GetNextMaintenanceId()
        {
            int id = await _unitOfWork.GetRepository<Maintenance>().Entities.AnyAsync()
                ? await _unitOfWork.GetRepository<Maintenance>().Entities.MaxAsync(_ => _.MaintenanceId)
                : 0;
            return id + 1;
        }

        public async Task AddMaintainService(CreateMaintenanceDTO dto)
        {
            Account createdBy = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(_ => _.Username == dto.CreatedBy && !_.IsDeleted)
                ?? throw new BusinessException("Tài khoản không tồn tại");
            Department department = await _unitOfWork.GetRepository<Department>().Entities.FirstOrDefaultAsync(_ => _.DepartmentName == dto.Department && !_.IsDeleted)
                ?? throw new BusinessException("Bộ phận không tồn tại");

            Maintenance maintenance = new()
            {
                MaintenanceId = dto.MaintenanceId,
                MaintenanceName = dto.MaintenanceName,
                MaintanaceDate = dto.MaintanaceDate,
                CreatedBy = createdBy.AccountId,
                DepartmentId = department.DepartmentId,
                Description = dto.Description,
                Status = "Chưa hoàn thành"
            };

            foreach (string equipmentId in dto.EquipmentId)
            {
                Equipment equipment = await _unitOfWork.GetRepository<Equipment>().Entities.FirstOrDefaultAsync(_ => _.EquipmentId == int.Parse(equipmentId) && !_.IsDeleted)
                    ?? throw new BusinessException($"Thiết bị có mã {equipmentId} không tồn tại");
                maintenance.Equipment.Add(equipment);
            }

            await _unitOfWork.GetRepository<Maintenance>().InsertAsync(maintenance);
            await _unitOfWork.SaveAsync();
        }
    }

    public class ResponseEquipment
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; } = null!;
    }
}
