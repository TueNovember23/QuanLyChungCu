using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.AccountDTO;

namespace Services.Services.AdministrativeStaffServices
{
    public class AccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseAccountDTO>> GetAll()
        {
            List<ResponseAccountDTO> result = await _unitOfWork.GetRepository<Account>().Entities.Where(_ => _.IsDeleted == false).Select(_ => new ResponseAccountDTO
            {
                AccountId = _.AccountId,
                Username = _.Username,
                Password = _.Password,
                FullName = _.FullName,
                Role = _.Role.RoleName,
                IsDeleted = _.IsDeleted
            }).ToListAsync();
            return result;
        }
    }
}
