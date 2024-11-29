using Services.DTOs.GeneralInfo.AreaDTO;
using Services.DTOs.GeneralInfo.BlockDTO;
using Services.DTOs.GeneralInfo.FloorDTO;
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

        // -------------------
        Task<IEnumerable<BlockResponseDTO>> GetBlocksByAreaAsync(int areaId);
        Task<IEnumerable<BlockResponseDTO>> SearchBlocksAsync(int areaId, string searchText);
        // -------------------
        Task<IEnumerable<FloorResponseDTO>> GetFloorsByBlockAsync(int blockId);
        Task<IEnumerable<FloorResponseDTO>> SearchFloorsAsync(int blockId, string searchText);
    }
}
