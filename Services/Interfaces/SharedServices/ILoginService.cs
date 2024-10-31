using Services.DTOs.LoginDTO;

namespace Services.Interfaces.SharedServices
{
    public interface ILoginService
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO request);
    }
}
