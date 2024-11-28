using Core;

namespace Services.DTOs.ResidentDTO
{
    public class CreateResidentDTO
    {
        public string ResidentId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? RelationShipWithOwner { get; set; }

        public string ApartmentCode { get; set; } = null!;

        public void Validate()
        {
            if (string.IsNullOrEmpty(ResidentId) || string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(ApartmentCode))
            {
                throw new BusinessException("Vui lòng điền đầy đủ thông tin!");
            }
        }
    }
}
