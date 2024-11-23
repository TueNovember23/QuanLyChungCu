using AutoMapper;
using Core;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.RegulationDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Services.SharedServices;

namespace Services.Services.AdministrativeStaffServices
{
    public class RegulationService : IRegulationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegulationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<RegulationDTO>> GetAllRegulationsAsync()
        {
            var regulations = await _unitOfWork.GetRepository<Regulation>().GetAllAsync();
            return _mapper.Map<List<RegulationDTO>>(regulations);
        }

        public async Task<RegulationDTO> GetRegulationByIdAsync(int id)
        {
            var regulation = await _unitOfWork.GetRepository<Regulation>().GetByIdAsync(id);
            return _mapper.Map<RegulationDTO>(regulation);
        }

        public async Task<RegulationDTO> CreateRegulationAsync(RegulationDTO regulationDto)
        {
            try
            {
                // 1. Validate input
                if (string.IsNullOrWhiteSpace(regulationDto.Title))
                    throw new BusinessException("Tiêu đề không được để trống");
                
                if (string.IsNullOrWhiteSpace(regulationDto.Content))
                    throw new BusinessException("Nội dung không được để trống");
                    
                if (!RegulationConstantsService.Categories.Contains(regulationDto.Category))
                    throw new BusinessException("Phân loại không hợp lệ");
                    
                if (!RegulationConstantsService.PriorityLevels.Contains(regulationDto.Priority))
                    throw new BusinessException("Mức độ ưu tiên không hợp lệ");

                // Check for duplicate title
                var existingRegulation = (await _unitOfWork.GetRepository<Regulation>().GetAllAsync())
                    .FirstOrDefault(r => r.Title.Equals(regulationDto.Title, StringComparison.OrdinalIgnoreCase));
                    
                if (existingRegulation != null)
                    throw new BusinessException("Đã tồn tại nội quy với tiêu đề này");

                // 2. Create metadata object
                var metadata = new RegulationMetadata
                {
                    Category = regulationDto.Category,
                    Priority = regulationDto.Priority,
                    IsActive = regulationDto.IsActive,
                    Content = regulationDto.Content
                };

                // 3. Create and save entity
                var regulation = new Regulation
                {
                    Title = regulationDto.Title,
                    Content = JsonSerializer.Serialize(metadata),
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now)
                };

                await _unitOfWork.GetRepository<Regulation>().InsertAsync(regulation);
                await _unitOfWork.SaveAsync();

                // 4. Map back to DTO with metadata
                var savedMetadata = JsonSerializer.Deserialize<RegulationMetadata>(regulation.Content);
                var resultDto = _mapper.Map<RegulationDTO>(regulation);
                
                // 5. Set metadata properties
                resultDto.Category = savedMetadata.Category;
                resultDto.Priority = savedMetadata.Priority;
                resultDto.IsActive = savedMetadata.IsActive;
                resultDto.Content = savedMetadata.Content;

                return resultDto;
            }
            catch (JsonException)
            {
                throw new BusinessException("Lỗi xử lý dữ liệu nội quy");
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Lỗi khi tạo nội quy: {ex.Message}");
            }
        }

        public async Task<RegulationDTO> UpdateRegulationAsync(RegulationDTO regulationDto)
        {
            var regulation = await _unitOfWork.GetRepository<Regulation>().GetByIdAsync(regulationDto.RegulationId);
            if (regulation == null)
                throw new BusinessException("Không tìm thấy nội quy");

            regulation.Title = regulationDto.Title;
            regulation.Content = regulationDto.Content;

            await _unitOfWork.SaveAsync();
            return _mapper.Map<RegulationDTO>(regulation);
        }

        public async Task<bool> DeleteRegulationAsync(int id)
        {
            var regulation = await _unitOfWork.GetRepository<Regulation>().GetByIdAsync(id);
            if (regulation == null)
                return false;

            // Check for violations
            if (regulation.Violations.Any())
                throw new BusinessException("Không thể xóa nội quy đã có vi phạm");

            await _unitOfWork.GetRepository<Regulation>().DeleteAsync(regulation);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<List<RegulationDTO>> SearchRegulationsAsync(string searchText, string category)
        {
            var regulations = await _unitOfWork.GetRepository<Regulation>().GetAllAsync();
            var dtos = _mapper.Map<List<RegulationDTO>>(regulations);

            return dtos.Where(r =>
                (string.IsNullOrEmpty(searchText) ||
                 r.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(category) ||
                 r.Category == category)
            ).ToList();
        }

        public async Task<byte[]> ExportToPdfAsync(RegulationDTO regulation)
        {
            // Implement PDF export logic here
            throw new NotImplementedException();
        }
    }
}
