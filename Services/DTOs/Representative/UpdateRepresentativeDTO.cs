namespace Services.DTOs.Representative
{
    public class UpdateRepresentativeDTO
    {
        public string? FullName { get; set; }

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
