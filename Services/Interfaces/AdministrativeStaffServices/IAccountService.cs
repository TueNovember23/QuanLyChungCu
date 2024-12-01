using Services.DTOs.AccountDTO;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IAccountService
    {
        public Task<List<ResponseAccountDTO>> GetAll();

    }
}
