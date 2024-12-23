﻿using AutoMapper;
using Core;
using Repositories.Entities;
using Repositories.Interfaces;
using Services.DTOs.LoginDTO;
using Services.Interfaces.SharedServices;

namespace Services.Services.SharedServices
{
    public class LoginService(IUnitOfWork unitOfWork, IMapper mapper) : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        public async Task<LoginResponseDTO> Login(LoginRequestDTO request)
        {
            request.Validate();
            Account account = await _unitOfWork.GetRepository<Account>()
                .FindAsync(x => x.Username == request.Username && x.Password == request.Password)
                ?? throw new BusinessException("Invalid username or password");

            return _mapper.Map<LoginResponseDTO>(account);
        }
    }
}
