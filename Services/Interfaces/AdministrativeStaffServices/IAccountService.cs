using Repositories.Repositories.Entities;
using Services.DTOs.AccountDTO;

namespace Services.Interfaces.AdministrativeStaffServices
{
    public interface IAccountService
    {
        public Task<List<ResponseAccountDTO>> GetAll();
        public Task<List<ResponseAccountDTO>> Search(string searchText);
        public Task Delete(string username);
        public Task<List<Role>> GetAllRole();
        public Task Create(CreateAccountDTO dto);
        public Task UpdateAccount(string username, UpdateAccountDTO dto);

    }
}
