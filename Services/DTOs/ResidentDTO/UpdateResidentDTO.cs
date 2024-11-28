using Core;

namespace Services.DTOs.ResidentDTO
{
    public class UpdateResidentDTO
    {
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string RelationShipWithOwner { get; set; } = null!;
        
        public void Validate()
        {
            if (string.IsNullOrEmpty(FullName))
            {
                throw new BusinessException("Họ tên không được để trống");
            }
            else if (string.IsNullOrEmpty(Gender))
            {
                throw new BusinessException("Giới tính không được để trống");
            }
            else if (string.IsNullOrEmpty(RelationShipWithOwner))
            {
                throw new BusinessException("Quan hệ với chủ hộ không được để trống");
            }
        }
    }
}
