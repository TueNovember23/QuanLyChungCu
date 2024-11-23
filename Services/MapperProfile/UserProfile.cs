using AutoMapper;
using Repositories.Repositories.Entities;
using Services.DTOs.LoginDTO;

namespace Services.MapperProfile
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<LoginRequestDTO, Account>();
            CreateMap<Account, LoginResponseDTO>();
        }
    }
}
