using Services.DTOs.DepartmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDTO>> GetAllAsync();
        Task<DepartmentDTO?> GetByIdAsync(int id);
        Task<DepartmentDTO> CreateAsync(CreateDepartmentDTO dto);
        Task<DepartmentDTO> UpdateAsync(UpdateDepartmentDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<DepartmentDTO>> SearchAsync(string keyword);
    }
}
