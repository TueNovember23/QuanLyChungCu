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
        private readonly IRegulationRepository _regulationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RegulationService(IRegulationRepository regulationRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _regulationRepository = regulationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RegulationResponseDTO>> GetAllAsync()
        {
            var regulations = await _regulationRepository.GetRegulaAllAsync();
            return _mapper.Map<IEnumerable<RegulationResponseDTO>>(regulations);
        }

        public async Task<RegulationResponseDTO?> GetByIdAsync(int id)
        {
            var regulation = await _regulationRepository.GetRegulaByIdAsync(id);
            return _mapper.Map<RegulationResponseDTO>(regulation);
        }

        public async Task<IEnumerable<RegulationResponseDTO>> SearchAsync(string searchText)
        {
            var regulations = await _regulationRepository.SearchRegulaAsync(searchText);
            return _mapper.Map<IEnumerable<RegulationResponseDTO>>(regulations);
        }

        public async Task<RegulationResponseDTO> CreateAsync(CreateRegulationDTO dto)
        {
            if (!dto.IsValid())
                throw new BusinessException("Dữ liệu không hợp lệ");

            var regulation = _mapper.Map<Regulation>(dto);
            regulation.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _regulationRepository.AddRegulaAsync(regulation);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<RegulationResponseDTO>(regulation);
        }

        public async Task<RegulationResponseDTO> UpdateAsync(RegulationResponseDTO dto)
        {
            var regulation = await _regulationRepository.GetRegulaByIdAsync(dto.RegulationId);
            if (regulation == null)
                throw new BusinessException("Không tìm thấy nội quy");

            _mapper.Map(dto, regulation);
            _regulationRepository.UpdateRegula(regulation);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<RegulationResponseDTO>(regulation);
        }

        public async Task DeleteAsync(int id)
        {
            await _regulationRepository.DeleteRegulaAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public Task<List<string>> GetCategoriesAsync()
        {
            return Task.FromResult(RegulationConstantsService.Categories);
        }

        public Task<List<string>> GetPriorityLevelsAsync()
        {
            return Task.FromResult(RegulationConstantsService.PriorityLevels);
        }
    }
}
