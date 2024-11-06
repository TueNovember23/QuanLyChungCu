using Services.DTOs.ApartmentDTO;

namespace Services.Interfaces.AccountantServices
{
    public interface IApartmentService
    {
        public Task<List<ResponseApartmentDTO>> GetAll();
    }
}
