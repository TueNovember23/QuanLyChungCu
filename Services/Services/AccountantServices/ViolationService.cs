using AutoMapper;
using Core;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.RegulationDTO;
using Services.DTOs.ViolationDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountantServices
{
    public class ViolationService : IViolationService
    {
        private readonly IViolationRepository _violationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ViolationService(IViolationRepository violationRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _violationRepository = violationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ViolationResponseDTO>> GetAllAsync()
        {
            var violations = await _violationRepository.GetViolatAllAsync();
            return _mapper.Map<IEnumerable<ViolationResponseDTO>>(violations);
        }

        public async Task<ViolationResponseDTO?> GetByIdAsync(int id)
        {
            var violation = await _violationRepository.GetViolatByIdAsync(id);
            return _mapper.Map<ViolationResponseDTO>(violation);
        }

        public async Task<IEnumerable<ViolationResponseDTO>> SearchAsync(string searchText)
        {
            try
            {
                var violations = await _violationRepository.SearchViolatAsync(searchText);
                return _mapper.Map<IEnumerable<ViolationResponseDTO>>(violations);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Lỗi khi tìm kiếm: {ex.Message}");
            }
        }

        public async Task<ViolationResponseDTO> CreateAsync(CreateViolationDTO dto)
        {
            if (!dto.IsValid())
                throw new BusinessException("Dữ liệu không hợp lệ");

            var violation = _mapper.Map<Violation>(dto);
            await _violationRepository.AddViolatAsync(violation);
            await _unitOfWork.SaveAsync();

            violation = await _violationRepository.GetViolatByIdAsync(violation.ViolationId);
            return _mapper.Map<ViolationResponseDTO>(violation);
        }

        public async Task<ViolationResponseDTO> UpdateAsync(CreateViolationDTO dto, int id)
        {
            if (!dto.IsValid())
                throw new BusinessException("Dữ liệu không hợp lệ");

            var violation = await _violationRepository.GetViolatByIdAsync(id);
            if (violation == null)
                throw new BusinessException("Không tìm thấy vi phạm");

            _mapper.Map(dto, violation);
            _violationRepository.UpdateViolat(violation);
            await _unitOfWork.SaveAsync();

            violation = await _violationRepository.GetViolatByIdAsync(id);
            return _mapper.Map<ViolationResponseDTO>(violation);
        }

        public async Task DeleteAsync(int id)
        {
            var violation = await _violationRepository.GetViolatByIdAsync(id);
            if (violation == null)
                throw new BusinessException("Không tìm thấy vi phạm");

            await _violationRepository.DeleteViolatAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> SavePenaltyAsync(ViolationPenaltyDTO penaltyDTO)
        {
            if (!penaltyDTO.IsValid())
                throw new BusinessException("Dữ liệu xử lý không hợp lệ");

            var existingPenalty = await _violationRepository
                .GetPenaltiesByViolationIdAsync(penaltyDTO.ViolationId);
            
            if (existingPenalty.Any(p => 
                p.ProcessedDate?.Date == penaltyDTO.ProcessedDate.Date))
            {
                throw new BusinessException("Đã tồn tại xử lý trong ngày này");
            }

            try
            {
                // Kiểm tra vi phạm tồn tại
                var violation = await _violationRepository.GetViolatByIdAsync(penaltyDTO.ViolationId);
                if (violation == null)
                    throw new BusinessException("Không tìm thấy vi phạm");

                var penalty = _mapper.Map<ViolationPenalty>(penaltyDTO);
                await _violationRepository.AddPenaltyAsync(penalty);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Lỗi khi lưu xử lý: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ViolationPenaltyDTO>> GetPenaltyHistoryAsync(int violationId)
        {
            var penalties = await _violationRepository.GetPenaltiesByViolationIdAsync(violationId);
            return _mapper.Map<IEnumerable<ViolationPenaltyDTO>>(penalties); 
        }

        public async Task<bool> UpdatePenaltyAsync(ViolationPenaltyDTO penaltyDTO)
        {
            var violation = await _violationRepository.GetViolatByIdAsync(penaltyDTO.ViolationId);
            if (violation == null)
                throw new BusinessException("Không tìm thấy vi phạm");

            if (!penaltyDTO.IsValid())
                throw new BusinessException("Dữ liệu xử lý không hợp lệ");

            try
            {
                var penalty = await _violationRepository.GetPenaltyByIdAsync(penaltyDTO.PenaltyId);
                if (penalty == null)
                    throw new BusinessException("Không tìm thấy thông tin xử lý");

                _mapper.Map(penaltyDTO, penalty);
                await _violationRepository.UpdatePenaltyAsync(penalty);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Lỗi khi cập nhật xử lý: {ex.Message}");
            }
        }

        public async Task<bool> HasActiveViolationsForRegulation(int regulationId)
        {
            try
            {
                var violations = await _violationRepository.GetViolationsByRegulationId(regulationId);
                return violations.Any(v => v.ViolationPenalties.Any(p => 
                    p.ProcessingStatus != "Đã xử lý")); 
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Lỗi kiểm tra vi phạm: {ex.Message}");
            }
        }
    }
}
