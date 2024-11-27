using Repositories.Repositories.Entities;
using Services.DTOs.EquipmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AccountantServices
{
    public interface IEquipmentService
    {
        Task<List<ResponseEquipmentDTO>> GetAll();
        Task<List<ResponseEquipmentDTO>> Search(string searchText);
        Task<Equipment?> GetEquipmentById(int id);
    }
}
