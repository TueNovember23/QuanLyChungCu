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
            if(Gender == "Nam" && (RelationShipWithOwner == "Vợ" || RelationShipWithOwner == "Mẹ" || RelationShipWithOwner == "Chị"
                || RelationShipWithOwner == "Cô" || RelationShipWithOwner == "Dì" || RelationShipWithOwner == "Thím" || RelationShipWithOwner == "Mợ" || RelationShipWithOwner == "Bà"))
            {
                throw new BusinessException("Mối quan hệ không phù hợp với giới tính");
            }
            if(Gender == "Nữ" && (RelationShipWithOwner == "Chồng" || RelationShipWithOwner == "Cha" || RelationShipWithOwner == "Anh"
                || RelationShipWithOwner == "Dượng" || RelationShipWithOwner == "Cậu" || RelationShipWithOwner == "Chú" || RelationShipWithOwner == "Ông"))
            {
                throw new BusinessException("Mối quan hệ không phù hợp với giới tính");
            }
            int age = DateTime.Now.Year - DateOfBirth!.Value.Year;
            if (age < 18 && (RelationShipWithOwner == "Vợ" || RelationShipWithOwner == "Chồng"))
            {
                throw new BusinessException("Mối quan hệ không phù hợp với độ tuổi");
            }
        }
    }
}
