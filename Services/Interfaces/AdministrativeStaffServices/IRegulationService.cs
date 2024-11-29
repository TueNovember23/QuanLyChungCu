using Services.DTOs.RegulationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IRegulationService
    {
        Task<IEnumerable<RegulationResponseDTO>> GetAllAsync();
        Task<RegulationResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<RegulationResponseDTO>> SearchAsync(string searchText);
        Task<RegulationResponseDTO> CreateAsync(CreateRegulationDTO dto);
        Task<RegulationResponseDTO> UpdateAsync(RegulationResponseDTO dto);
        Task DeleteAsync(int id);
        Task<List<string>> GetCategoriesAsync();
        Task<List<string>> GetPriorityLevelsAsync();
    }
}
