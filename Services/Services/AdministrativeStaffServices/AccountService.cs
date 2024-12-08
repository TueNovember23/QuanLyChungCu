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

        public async Task<List<Role>> GetAllRole()
        {
            List<Role> roles = await _unitOfWork.GetRepository<Role>().Entities.ToListAsync();
            return roles;
        }

        public async Task Delete(string username)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(_ => _.Username == username)
                ?? throw new BusinessException($"Không tìm thấy tài khoản {username}");
            account.IsDeleted = true;
            await _unitOfWork.SaveAsync();
        }

        public async Task Create(CreateAccountDTO dto)
        {
            Account? account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(_ => _.Username == dto.Username);
            if (account != null)
            {
                throw new BusinessException($"Tên tài khoản {dto.Username} đã tồn tại");
            }
            Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(_ => _.RoleName == dto.Role)
                ?? throw new BusinessException($"Không tìm thấy role {dto.Role}");
            Account newAccount = new Account
            {
                Username = dto.Username,
                Password = dto.Password,
                FullName = dto.FullName,
                RoleId = role.RoleId
            };
            await _unitOfWork.GetRepository<Account>().InsertAsync(newAccount);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAccount(string username, UpdateAccountDTO dto)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(_ => _.Username == username)
                 ?? throw new BusinessException($"Không tìm thấy tài khoản {username}");
            if(string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.ComfirmPassword))
            {
                throw new BusinessException("Vui lòng điền đầy đủ thông tin");
            }
            if (dto.Password != dto.ComfirmPassword)
            {
                throw new BusinessException("Mật khẩu không khớp");
            }
            account.Password = dto.Password;
            await _unitOfWork.SaveAsync();
        }
    }
}
