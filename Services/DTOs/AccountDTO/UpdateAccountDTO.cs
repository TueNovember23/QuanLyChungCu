using Core;

namespace Services.DTOs.AccountDTO
{
    public class UpdateAccountDTO
    {
        public string Password { get; set; } = null!;
        public string ComfirmPassword { get; set; } = null!;
    }
}
