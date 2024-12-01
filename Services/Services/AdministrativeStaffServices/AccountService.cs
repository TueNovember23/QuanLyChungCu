using Core;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.AccountDTO;
using Services.Interfaces.AdministrativeStaffServices;

namespace Services.Services.AdministrativeStaffServices
{
    public class AccountService : IAccountService
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

        public async Task<List<ResponseAccountDTO>> Search(string searchText)
        {
            List<ResponseAccountDTO> result = await _unitOfWork.GetRepository<Account>().Entities
                .Where(_ => _.IsDeleted == false
                && (_.Username.Contains(searchText) ||
                _.FullName.Contains(searchText) ||
                _.Role.RoleName.Contains(searchText)))
                .Select(_ => new ResponseAccountDTO
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

        public async Task Delete(string username)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(_ => _.Username == username)
                ?? throw new BusinessException($"Không tìm thấy tài khoản {username}");
            account.IsDeleted = true;
            await _unitOfWork.SaveAsync();
        }
    }
}
