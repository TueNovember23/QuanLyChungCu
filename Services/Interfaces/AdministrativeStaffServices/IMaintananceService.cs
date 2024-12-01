﻿using Repositories.Repositories.Entities;
using Services.DTOs.MaintenanceDTO;
using Services.Services.AdministrativeStaffServices;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IMaintananceService
    {
        public Task<List<ResponseMaintenance>> GetAll();
        public Task<List<ResponseMaintenance>> Search(string searchText);
        public Task CompleteMaintenance(int id);
        public Task<List<ResponseEquipment>> GetAllEquipment();
        public Task<int> GetNextMaintenanceId();
    }
}
