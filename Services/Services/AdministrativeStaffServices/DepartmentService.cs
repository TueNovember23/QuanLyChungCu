using AutoMapper;
using Core;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.DepartmentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AdministrativeStaffServices
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetDepartAllAsync();
            return _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
        }

        public async Task<DepartmentDTO?> GetByIdAsync(int id)
        {
            var department = await _departmentRepository.GetDepartByIdAsync(id);
            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task<DepartmentDTO> CreateAsync(CreateDepartmentDTO dto)
        {
            if (!dto.IsValid())
                throw new BusinessException("Invalid department data");

            var existingDepartment = await _departmentRepository.GetDepartByNameAsync(dto.DepartmentName);
            if (existingDepartment != null)
                throw new BusinessException("Department name already exists");

            var department = _mapper.Map<Department>(dto);
            await _departmentRepository.AddDepartAsync(department);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task<DepartmentDTO> UpdateAsync(UpdateDepartmentDTO dto)
        {
            if (!dto.IsValid())
                throw new BusinessException("Invalid department data");

            var department = await _departmentRepository.GetDepartByIdAsync(dto.DepartmentId);
            if (department == null)
                throw new BusinessException("Department not found");

            var existingDepartment = await _departmentRepository.GetDepartByNameAsync(dto.DepartmentName);
            if (existingDepartment != null && existingDepartment.DepartmentId != dto.DepartmentId)
                throw new BusinessException("Department name already exists");

            _mapper.Map(dto, department);
            _departmentRepository.UpdateDepart(department);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _departmentRepository.GetDepartByIdAsync(id);
            if (department == null)
                throw new BusinessException("Department not found");

            _departmentRepository.DeleteDepart(department);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<DepartmentDTO>> SearchAsync(string keyword)
        {
            var departments = await _departmentRepository.SearchDepartAsync(keyword);
            return _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
        }
    }
}
