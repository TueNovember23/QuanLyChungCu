using Core;

namespace Services.DTOs.LoginDTO
{
    public class LoginRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new BusinessException("Username is required");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentException("Password is required");
            }
        }
    }
}
