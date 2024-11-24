using Services.DTOs.ApartmentDTO;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IApartmentService
    {
        public Task<List<ResponseApartmentDTO>> GetAll();
        public Task<List<ResponseApartmentDTO>> Search(string searchText);
    }
}
