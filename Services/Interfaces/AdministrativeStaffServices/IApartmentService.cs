using Repositories.Repositories.Entities;
using Services.DTOs.ApartmentDTO;
using Services.DTOs.ResidentDTO;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IApartmentService
    {
        public Task<List<ResponseApartmentDTO>> GetAll();
        public Task<List<ResponseApartmentDTO>> Search(string searchText);
        public Task<Apartment?> GetApartmentByCode(string code);
        public Task<List<ResponseResidentDTO>> GetResidentsOfApartment(string apartmentCode);
        public Task RegisterResident(CreateResidentDTO createDTO);
        public Task<List<ResponseApartmentDTO>> GetAllApartmentForViolation();
        public Task UpdateResident(string id, UpdateResidentDTO dto);
        public Task MoveResidentOut(string residentId);
        public Task MoveResidentIn(string residentId);
        public Task<Resident> GetResidentById(string id);
    }
}
