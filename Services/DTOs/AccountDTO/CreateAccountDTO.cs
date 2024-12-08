using Core;

namespace Services.DTOs.AccountDTO
{
    public class CreateAccountDTO
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Role { get; set; } = null!;

        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrEmpty(Role))
            {
                throw new BusinessException("Vui lòng điền đầy đủ thông tin");
            }
            if(Password != ConfirmPassword)
            {
                throw new BusinessException("Mật khẩu không khớp");
            }
        }
    }
}
