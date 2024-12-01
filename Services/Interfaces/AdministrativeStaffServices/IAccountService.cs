using Services.DTOs.AccountDTO;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IAccountService
    {
        public Task<List<ResponseAccountDTO>> GetAll();
        public Task<List<ResponseAccountDTO>> Search(string searchText);
        public Task Delete(string username);

    }
}
