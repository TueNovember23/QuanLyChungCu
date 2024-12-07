using Services.DTOs.ApartmentDTO;
using Services.DTOs.RegulationDTO;
using Services.DTOs.ViolationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AccountantServices
{
    public interface IViolationService
    {
        Task<IEnumerable<ViolationResponseDTO>> GetAllAsync();
        Task<ViolationResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ViolationResponseDTO>> SearchAsync(string searchText);
        Task<ViolationResponseDTO> CreateAsync(CreateViolationDTO dto);
        Task<ViolationResponseDTO> UpdateAsync(CreateViolationDTO dto, int id);
        Task DeleteAsync(int id);

        Task<bool> SavePenaltyAsync(ViolationPenaltyDTO penaltyDTO);
        Task<bool> UpdatePenaltyAsync(ViolationPenaltyDTO penaltyDTO);
        Task<IEnumerable<ViolationPenaltyDTO>> GetPenaltyHistoryAsync(int violationId);
        Task<bool> HasActiveViolationsForRegulation(int regulationId);
    }
}
