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
    // Services/Services/AccountantServices/ViolationService.cs
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
            var violations = await _violationRepository.SearchViolatAsync(searchText);
            return _mapper.Map<IEnumerable<ViolationResponseDTO>>(violations);
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
    }
}
