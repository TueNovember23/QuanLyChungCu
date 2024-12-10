using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.Entities;
using Repositories.Interfaces;
using Services.DTOs.ApartmentDTO;
using Services.Interfaces.AdministrativeStaffServices;
using AutoMapper;
using Core;
using Services.DTOs.ResidentDTO;
using Azure.Core;
using Services.DTOs.Representative;
using Services.DTOs.HouseholdMovementDTO;

namespace Services.Services.AdministrativeStaffServices
{
    public class ApartmentService(IUnitOfWork unitOfWork) : IApartmentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<ResponseApartmentDTO>> GetAll()
        {
            List<ResponseApartmentDTO> apartments = await _unitOfWork.GetRepository<Apartment>().Entities
                .Select(Apartment => new ResponseApartmentDTO
                {
                    ApartmentCode = Apartment.ApartmentCode!,
                    Area = Apartment.Area,
                    NumberOfPeople = Apartment.NumberOfPeople,
                    Block = Apartment.Floor.Block.BlockCode,
                    Floor = Apartment.Floor.FloorNumber,
                    Status = Apartment.Status
                }).ToListAsync();

            return apartments;
        }

        public async Task<List<ResponseApartmentDTO>> Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await GetAll();
            }

            var query = _unitOfWork.GetRepository<Apartment>().Entities
                .Include(apartment => apartment.Floor.Block)
                .Include(apartment => apartment.Representative)
                .Where(apartment =>
                    (apartment.ApartmentCode != null && apartment.ApartmentCode.Contains(searchText)) ||
                    (apartment.Floor.Block.BlockCode != null && apartment.Floor.Block.BlockCode.Contains(searchText)) ||
                    apartment.Floor.FloorNumber.ToString().Contains(searchText) ||
                    apartment.Residents.Any(r =>
                        (r.FullName != null && r.FullName.Contains(searchText)) ||
                        (r.ResidentId != null && r.ResidentId.Contains(searchText))) ||
                    (apartment.Status != null && apartment.Status.Contains(searchText)) ||
                    (apartment.Representative != null && apartment.Representative.FullName != null && apartment.Representative.FullName.Contains(searchText))
                );

            var response = await query.Select(apartment => new ResponseApartmentDTO
            {
                ApartmentCode = apartment.ApartmentCode!,
                Area = apartment.Area,
                NumberOfPeople = apartment.NumberOfPeople,
                Block = apartment.Floor.Block.BlockCode,
                Floor = apartment.Floor.FloorNumber,
                Status = apartment.Status
            }).ToListAsync();

            return response;
        }

        public async Task<Apartment?> GetApartmentByCode(string code)
        {
            return await _unitOfWork.GetRepository<Apartment>().Entities.FirstOrDefaultAsync(_ => _.ApartmentCode == code);
        }

        public async Task<List<ResponseResidentDTO>> GetResidentsOfApartment(string apartmentCode)
        {
            List<ResponseResidentDTO> residents = [];
            residents = await _unitOfWork.GetRepository<Resident>().Entities
                .Where(_ => _.Apartment.ApartmentCode == apartmentCode)
                .Select(r => new ResponseResidentDTO
                {
                    ResidentId = r.ResidentId,
                    FullName = r.FullName,
                    Gender = r.Gender,
                    DateOfBirth = r.DateOfBirth.HasValue ? r.DateOfBirth.Value.ToString("dd/MM/yyyy") : null,
                    RelationShipWithOwner = r.RelationShipWithOwner,
                    MoveInDate = r.MoveInDate.HasValue ? r.MoveInDate.Value.ToString("dd/MM/yyyy") : null,
                    MoveOutDate = r.MoveOutDate.HasValue ? r.MoveOutDate.Value.ToString("dd/MM/yyyy") : null
                })
                .OrderByDescending(r => r.RelationShipWithOwner == "Chủ hộ")
                .ToListAsync();
            return residents;
        }


        public async Task RegisterResident(CreateResidentDTO createDTO)
        {
            if (createDTO == null)
            {
                throw new BusinessException("Thông tin đăng ký không hợp lệ");
            }

            createDTO.Validate();
            Apartment apartment = _unitOfWork.GetRepository<Apartment>().Entities
                .Where(a => a.ApartmentCode == createDTO.ApartmentCode)
                .FirstOrDefault()
                ?? throw new BusinessException("Căn hộ không tồn tại");

            Resident? existingResident = _unitOfWork.GetRepository<Resident>().Entities
                .Where(r => r.ResidentId == createDTO.ResidentId && r.MoveOutDate == null)
                .FirstOrDefault();
            if (existingResident is not null)
            {
                throw new BusinessException($"Cư dân hiện vẫn đang ở tại căn hộ {existingResident.Apartment.ApartmentCode}, hãy chuyển cư dân ra khỏi căn hộ trước khi đăng ký");
            }

            Resident newResident = new()
            {
                ResidentId = createDTO.ResidentId,
                FullName = createDTO.FullName,
                Gender = createDTO.Gender,
                DateOfBirth = createDTO.DateOfBirth,
                RelationShipWithOwner = createDTO.RelationShipWithOwner,
                MoveInDate = DateOnly.FromDateTime(DateTime.Now),
                MoveOutDate = null,
                ApartmentId = apartment.ApartmentId,
            };
            HouseholdMovement newMovement = new()
            {
                ResidentId = newResident.ResidentId,
                MovementType = "Chuyển vào",
                MovementDate = DateOnly.FromDateTime(DateTime.Now),
                ApartmentId = newResident.ApartmentId
            };
            await _unitOfWork.GetRepository<Resident>().InsertAsync(newResident);
            await _unitOfWork.GetRepository<HouseholdMovement>().InsertAsync(newMovement);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Resident> GetResidentById(string id)
        {
            return await _unitOfWork.GetRepository<Resident>().Entities.Where(_ => _.ResidentId == id).FirstOrDefaultAsync()
                ?? throw new BusinessException($"Không tìm thấy người dùng có số căn cước {id}");
        }

        public async Task UpdateResident(string id, UpdateResidentDTO dto)
        {
            Resident resident = await _unitOfWork.GetRepository<Resident>().GetByIdAsync(id)
                ?? throw new BusinessException($"Không tim thấy thông tin cư dân {id}");
            dto.Validate();
            resident.FullName = dto.FullName;
            resident.DateOfBirth = dto.DateOfBirth;
            resident.Gender = dto.Gender;
            if (dto.RelationShipWithOwner.Trim() == "Chủ hộ")
            {
                bool existed = await _unitOfWork.GetRepository<Resident>().Entities
                    .AnyAsync(r => r.ResidentId != resident.ResidentId && r.ApartmentId == resident.ApartmentId && r.RelationShipWithOwner == "Chủ hộ");
                if (existed) { throw new BusinessException("Căn hộ đã có chủ hộ"); }
            }
            resident.RelationShipWithOwner = dto.RelationShipWithOwner;
            await _unitOfWork.SaveAsync();
        }

        public async Task MoveResidentOut(string residentId)
        {
            Resident resident = await _unitOfWork.GetRepository<Resident>().Entities.Include(_ => _.Apartment).FirstOrDefaultAsync(_ => _.ResidentId == residentId)
                ?? throw new BusinessException($"Không tìm thấy cư dân có mã số căn cước {residentId}");
            if (resident.MoveOutDate != null)
            {
                throw new BusinessException($"Cư dân {resident.FullName} hiện không ở trong căn hộ {resident.Apartment.ApartmentCode}");
            }
            resident.MoveOutDate = DateOnly.FromDateTime(DateTime.Now);
            resident.Apartment.NumberOfPeople -= 1;
            HouseholdMovement newMovement = new()
            {
                ResidentId = resident.ResidentId,
                MovementType = "Chuyển ra",
                MovementDate = DateOnly.FromDateTime(DateTime.Now),
                ApartmentId = resident.ApartmentId
            };
            await _unitOfWork.GetRepository<HouseholdMovement>().InsertAsync(newMovement);
            await _unitOfWork.SaveAsync();
        }

        public async Task MoveResidentIn(string residentId)
        {
            Resident resident = await _unitOfWork.GetRepository<Resident>().GetByIdAsync(residentId)
               ?? throw new BusinessException($"Không tìm thấy cư dân có mã số căn cước {residentId}");
            if (resident.MoveOutDate == null)
            {
                throw new BusinessException($"Cư dân {resident.FullName} hiện đã ở trong căn hộ {resident.Apartment.ApartmentCode}");
            }
            resident.MoveOutDate = null;
            resident.MoveInDate = DateOnly.FromDateTime(DateTime.Now);
            resident.Apartment.NumberOfPeople += 1;
            HouseholdMovement newMovement = new()
            {
                ResidentId = resident.ResidentId,
                MovementType = "Chuyển vào",
                MovementDate = DateOnly.FromDateTime(DateTime.Now),
                ApartmentId = resident.ApartmentId
            };
            await _unitOfWork.GetRepository<HouseholdMovement>().InsertAsync(newMovement);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<Representative>> GetAllRepresentative()
        {
            return await _unitOfWork.GetRepository<Representative>().Entities.ToListAsync();
        }

        public async Task<Representative?> GetPreresentativeByApartmentCode(string apartmentCode)
        {
            Representative? representative = (await _unitOfWork.GetRepository<Apartment>().Entities
                .FirstOrDefaultAsync(_ => _.ApartmentCode == apartmentCode))?.Representative;
            return representative;
        }

        public async Task<Representative?> GetRepresentativeById(string? id)
        {
            Representative? representative = (await _unitOfWork.GetRepository<Representative>().Entities
                .FirstOrDefaultAsync(_ => _.RepresentativeId == id));
            return representative;
        }

        public async Task UpdateApartmentRepresentative(string apartmentCode, string reprensentativeId)
        {
            Apartment apartment = await _unitOfWork.GetRepository<Apartment>().Entities.FirstOrDefaultAsync(a => a.ApartmentCode == apartmentCode)
                ?? throw new BusinessException($"Không tìm thấy căn hộ {apartmentCode}");
            Representative representative = await _unitOfWork.GetRepository<Representative>().Entities.FirstOrDefaultAsync(r => r.RepresentativeId == reprensentativeId)
                ?? throw new BusinessException($"Không tìm thấy người đại diện có mã số căn cước {reprensentativeId}");
            apartment.RepresentativeId = reprensentativeId;
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateRepresentative(string id, UpdateRepresentativeDTO dto)
        {
            Representative representative = await _unitOfWork.GetRepository<Representative>().GetByIdAsync(id)
                ?? throw new BusinessException($"Không tìm thấy người đại diện có mã số căn cước {id}");
            dto.Validate();
            representative.FullName = dto.FullName;
            representative.DateOfBirth = dto.DateOfBirth;
            representative.Gender = dto.Gender;
            representative.Email = dto.Email;
            representative.PhoneNumber = dto.PhoneNumber;
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<ResponseApartmentDTO>> GetAllApartmentForViolation()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Apartment>();
                // Use ToListAsync() first to materialize the data before projecting
                var apartments = await repository.Entities
                    .AsNoTracking()
                    .Include(a => a.Floor)
                        .ThenInclude(f => f.Block)
                    .ToListAsync();

                var result = apartments.Select(apartment => new ResponseApartmentDTO
                {
                    ApartmentId = apartment.ApartmentId,
                    ApartmentCode = apartment.ApartmentCode!,
                    Area = apartment.Area,
                    NumberOfPeople = apartment.NumberOfPeople,
                    Block = apartment.Floor.Block.BlockCode,
                    Floor = apartment.Floor.FloorNumber,
                    Status = apartment.Status
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error getting apartments: {ex.Message}");
            }
        }

        public async Task<List<ResponseHouseholdMovementDTO>> GetMovementByApartmentCode(string apartmentCode)
        {
            List<ResponseHouseholdMovementDTO> list = await _unitOfWork.GetRepository<HouseholdMovement>().Entities
            .Where(movement => movement.Apartment.ApartmentCode == apartmentCode)
            .Select(movement => new ResponseHouseholdMovementDTO
            {
                Id = movement.MovementId,
                MovementDate = movement.MovementDate,
                ResidentId = movement.ResidentId,
                ResidentName = movement.Resident.FullName,
                MovementType = movement.MovementType
            }).OrderByDescending(_ => _.MovementDate).ThenByDescending(_ => _.Id).ToListAsync();
            return list;
        }

        public async Task<List<ResponseHouseholdMovementDTO>> GetMovementByApartmentCode(string apartmentCode, string searchText = "")
        {
            var query = _unitOfWork.GetRepository<HouseholdMovement>().Entities
                .Where(movement => movement.Apartment.ApartmentCode == apartmentCode);

            // Lọc dựa trên searchText
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower().Trim();

                if (DateOnly.TryParse(searchText, out var searchDate))
                {
                    query = query.Where(movement => movement.MovementDate == searchDate);
                }
                else
                {
                    query = query.Where(movement =>
                        movement.Resident.FullName.Contains(searchText) ||
                        movement.Resident.ResidentId.Contains(searchText) ||
                        movement.MovementType.Contains(searchText));
                }
            }

            List<ResponseHouseholdMovementDTO> list = await query
                .Select(movement => new ResponseHouseholdMovementDTO
                {
                    MovementDate = movement.MovementDate,
                    ResidentId = movement.ResidentId,
                    ResidentName = movement.Resident.FullName,
                    MovementType = movement.MovementType
                })
                .OrderByDescending(m => m.MovementDate).ThenByDescending(_ => _.Id)
                .ToListAsync();

            return list;
        }
    }
}
