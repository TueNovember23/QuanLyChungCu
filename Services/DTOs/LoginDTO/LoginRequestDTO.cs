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
                throw new BusinessException("Tên đăng nhập không được để trống");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentException("Mật khẩu không được để trống");
            }
        }
    }
}
