using Repositories.Repositories.Entities;

namespace Services.DTOs.HouseholdMovementDTO
{
    public class ResponseHouseholdMovementDTO
    {
        public int Id { get; set; }
        public DateOnly MovementDate { get; set; }

        public string ResidentId { get; set; } = null!;

        public string ResidentName { get; set; } = null!;

        public string ApartmentCode { get; set; } = null!;

        public string MovementType { get; set; } = null!;
    }
}
