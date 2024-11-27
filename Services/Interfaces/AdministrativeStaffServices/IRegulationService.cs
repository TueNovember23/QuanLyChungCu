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
        Task<List<RegulationDTO>> GetAllRegulationsAsync();
        Task<RegulationDTO> GetRegulationByIdAsync(int id);
        Task<RegulationDTO> CreateRegulationAsync(RegulationDTO regulationDto);
        Task<RegulationDTO> UpdateRegulationAsync(RegulationDTO regulationDto);
        Task<bool> DeleteRegulationAsync(int id);
        Task<byte[]> ExportToPdfAsync(RegulationDTO regulation);
        Task<List<RegulationDTO>> SearchRegulationsAsync(string searchText, string category);
    }
}
