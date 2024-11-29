namespace Services.DTOs.ResidentDTO
{
    public class CreateResidentDTO
    {
        public string ResidentId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? RelationShipWithOwner { get; set; }

        public int ApartmentCode { get; set; }
    }
}
