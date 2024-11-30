using Core;

namespace Services.DTOs.Representative
{
    public class UpdateRepresentativeDTO
    {
        public string? FullName { get; set; }

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Gender)
                || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                throw new BusinessException("Vui lòng điền đầy đủ thông tin");
            }

            if (!IsValidPhoneNumber(PhoneNumber))
            {
                throw new BusinessException("Số điện thoại không hợp lệ");
            }

            if (!IsValidEmail(Email))
            {
                throw new BusinessException("Email không hợp lệ");
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Kiểm tra độ dài và ký tự bắt đầu
            if (phoneNumber.Length < 10 || phoneNumber.Length > 11 || !phoneNumber.StartsWith("0"))
            {
                return false;
            }

            foreach (char c in phoneNumber)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
