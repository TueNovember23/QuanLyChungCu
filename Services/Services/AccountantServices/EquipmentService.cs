using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.EquipmentDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountantServices
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseEquipmentDTO>> GetAll()
        {
            var equipment = await _unitOfWork.GetRepository<Equipment>().Entities
                .Include(e => e.Area)
                .Include(e => e.Maintenances)
                .Where(e => !e.IsDeleted)
                .Select(e => new ResponseEquipmentDTO
                {
                    EquipmentId = e.EquipmentId,
                    EquipmentName = e.EquipmentName,
                    Description = e.Discription,
                    AreaName = e.Area.AreaName,
                    Status = e.Status,
                    LastMaintenanceDate = e.Maintenances
                        .OrderByDescending(m => m.MaintanaceDate)
                        .Select(m => m.MaintanaceDate.ToDateTime(TimeOnly.MinValue))
                        .FirstOrDefault()
                })
                .ToListAsync();

            return equipment;
        }

        public async Task<List<ResponseEquipmentDTO>> Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await GetAll();
            }

            var query = _unitOfWork.GetRepository<Equipment>().Entities
                .Include(e => e.Area)
                .Include(e => e.Maintenances)
                .Where(e => !e.IsDeleted &&
                    (e.EquipmentName.Contains(searchText) ||
                     e.Area.AreaName.Contains(searchText) ||
                     e.Status.Contains(searchText)))
                .Select(e => new ResponseEquipmentDTO
                {
                    EquipmentId = e.EquipmentId,
                    EquipmentName = e.EquipmentName,
                    Description = e.Discription,
                    AreaName = e.Area.AreaName,
                    Status = e.Status,
                    LastMaintenanceDate = e.Maintenances
                        .OrderByDescending(m => m.MaintanaceDate)
                        .Select(m => m.MaintanaceDate.ToDateTime(TimeOnly.MinValue))
                        .FirstOrDefault()
                });

            return await query.ToListAsync();
        }

        public async Task<Equipment?> GetEquipmentById(int id)
        {
            return await _unitOfWork.GetRepository<Equipment>().Entities
                .Include(e => e.Area)
                .Include(e => e.Maintenances)
                .FirstOrDefaultAsync(e => e.EquipmentId == id && !e.IsDeleted);
        }

        public async Task Add(ResponseEquipmentDTO equipmentDto)
        {
            var equipment = new Equipment
            {
                EquipmentName = equipmentDto.EquipmentName,
                Discription = equipmentDto.Description,
                Status = equipmentDto.Status,
                IsDeleted = false
            };

            var repository = _unitOfWork.GetRepository<Equipment>();
            await repository.InsertAsync(equipment);
            await _unitOfWork.SaveAsync();
        }

        public async Task Update(ResponseEquipmentDTO equipmentDto)
        {
            var equipment = await GetEquipmentById(equipmentDto.EquipmentId);
            if (equipment == null) return;

            equipment.EquipmentName = equipmentDto.EquipmentName;
            equipment.Discription = equipmentDto.Description;
            equipment.Area.AreaName = equipmentDto.AreaName;
            equipment.Status = equipmentDto.Status;

            var repository = _unitOfWork.GetRepository<Equipment>();
            repository.Update(equipment);
            await _unitOfWork.SaveAsync();
        }

        public async Task SoftDelete(int equipmentId)
        {
            var equipment = await GetEquipmentById(equipmentId);
            if (equipment == null) return;

            equipment.IsDeleted = true;
            var repository = _unitOfWork.GetRepository<Equipment>();
            repository.Update(equipment);
            await _unitOfWork.SaveAsync();
        }
    }
}
