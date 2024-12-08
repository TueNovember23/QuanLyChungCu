using Core;

namespace Services.DTOs.AccountDTO
{
    public class UpdateAccountDTO
    {
        public string Password { get; set; } = null!;
        public string ComfirmPassword { get; set; } = null!;

        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ComfirmPassword))
            {
                throw new BusinessException("Mật khẩu không được để trống");
            }
            if (Password != ComfirmPassword)
            {
                throw new BusinessException("Mật khẩu xác nhận không trùng khớp");
            }
        }
    }
}
