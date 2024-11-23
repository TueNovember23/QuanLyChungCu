using Services.DTOs.GeneralInfo.AreaDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IGeneralInfoService
    {
        Task<IEnumerable<AreaResponseDTO>> GetAllAreasAsync();
        Task<AreaResponseDTO> GetAreaByIdAsync(int id);
        Task<AreaResponseDTO> CreateAreaAsync(CreateAreaDTO dto);
        Task<AreaResponseDTO> UpdateAreaAsync(UpdateAreaDTO dto);
        Task DeleteAreaAsync(int id);
        Task<IEnumerable<AreaResponseDTO>> SearchAreasAsync(string searchText);
        Task AddAreaAsync(CreateAreaDTO createDto);
    }
}
