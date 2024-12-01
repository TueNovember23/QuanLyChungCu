﻿using AutoMapper;
using Core;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.GeneralInfo.AreaDTO;
using Services.DTOs.GeneralInfo.BlockDTO;
using Services.DTOs.GeneralInfo.FloorDTO;
using Services.Interfaces.AdministrativeStaffServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AdministrativeStaffServices
{
    public class GeneralInfoService : IGeneralInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GeneralInfoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AreaResponseDTO>> GetAllAreasAsync()
        {
            var areas = await _unitOfWork.GetRepository<Area>().GetAllAsync();
            return _mapper.Map<IEnumerable<AreaResponseDTO>>(areas);
        }

        public async Task<AreaResponseDTO> GetAreaByIdAsync(int id)
        {
            var repository = _unitOfWork.GetRepository<Area>();
            var area = await repository.Entities
                .Include(x => x.Blocks)
                .FirstOrDefaultAsync(x => x.AreaId == id);

            if (area == null)
                throw new BusinessException("Không tìm thấy khu vực này");

            return _mapper.Map<AreaResponseDTO>(area);
        }

        public async Task<AreaResponseDTO> CreateAreaAsync(CreateAreaDTO dto)
        {
            if (!dto.IsValid())
                throw new BusinessException("Thông tin khu vực không hợp lệ");

            var area = _mapper.Map<Area>(dto);
            await _unitOfWork.GetRepository<Area>().InsertAsync(area);
            await _unitOfWork.SaveAsync();

            return await GetAreaByIdAsync(area.AreaId);
        }

        public async Task<AreaResponseDTO> UpdateAreaAsync(UpdateAreaDTO dto)
        {
            if (!dto.IsValid())
                throw new BusinessException("Thông tin khu vực không hợp lệ");

            var repository = _unitOfWork.GetRepository<Area>();
            var area = await repository.Entities
                .FirstOrDefaultAsync(x => x.AreaId == dto.AreaId);

            if (area == null)
                throw new BusinessException("Không tìm thấy khu vực này");

            _mapper.Map(dto, area);
            await _unitOfWork.SaveAsync();

            return await GetAreaByIdAsync(area.AreaId);
        }

        public async Task DeleteAreaAsync(int id)
        {
            var repository = _unitOfWork.GetRepository<Area>();
            var area = await repository.Entities
                .Include(x => x.Blocks)
                .FirstOrDefaultAsync(x => x.AreaId == id);

            if (area == null)
                throw new BusinessException("Không tìm thấy khu vực này");

            if (area.Blocks.Any(b => (bool)!b.IsDeleted))
                throw new BusinessException("Không thể xóa khu vực đang có block");

            await repository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<AreaResponseDTO>> SearchAreasAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetAllAreasAsync();

            var repository = _unitOfWork.GetRepository<Area>();
            var areas = await repository.Entities
                .Include(x => x.Blocks)
                .Where(x => x.AreaName.Contains(searchText) ||
                           x.Location.Contains(searchText))
                .ToListAsync();

            return _mapper.Map<IEnumerable<AreaResponseDTO>>(areas);
        }

        public async Task AddAreaAsync(CreateAreaDTO createDto)
        {
            if (!createDto.IsValid())
                throw new BusinessException("Thông tin khu vực không hợp lệ");

            var area = _mapper.Map<Area>(createDto);
            await _unitOfWork.GetRepository<Area>().InsertAsync(area);
            await _unitOfWork.SaveAsync();
        }

        // -------------------

        public async Task<IEnumerable<BlockResponseDTO>> GetBlocksByAreaAsync(int areaId)
        {
            var blocks = await _unitOfWork.GetRepository<Block>().Entities
                .Where(b => b.AreaId == areaId && !(b.IsDeleted ?? false))
                .ToListAsync();
            return _mapper.Map<IEnumerable<BlockResponseDTO>>(blocks);
        }

        public async Task<IEnumerable<BlockResponseDTO>> SearchBlocksAsync(int areaId, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetBlocksByAreaAsync(areaId);

            var repository = _unitOfWork.GetRepository<Block>();
            var blocks = await repository.Entities
                .Where(b => b.AreaId == areaId 
                        && !(b.IsDeleted ?? false)
                        && b.BlockCode.Contains(searchText))
                .ToListAsync();
            return _mapper.Map<IEnumerable<BlockResponseDTO>>(blocks);
        }

        // -------------------
        public async Task<IEnumerable<FloorResponseDTO>> GetFloorsByBlockAsync(int blockId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Floor>();
                var floors = await repository.Entities
                    .Where(f => f.BlockId == blockId)
                    .ToListAsync();
                
                Debug.WriteLine($"Found {floors.Count} floors for block {blockId}");
                return _mapper.Map<IEnumerable<FloorResponseDTO>>(floors);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetFloorsByBlockAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<FloorResponseDTO>> SearchFloorsAsync(int blockId, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetFloorsByBlockAsync(blockId);

            var repository = _unitOfWork.GetRepository<Floor>();
            var floors = await repository.Entities
                .Where(f => f.BlockId == blockId && 
                    f.FloorNumber.ToString().Contains(searchText))
                .ToListAsync();
            return _mapper.Map<IEnumerable<FloorResponseDTO>>(floors);
        }

    }
}