using Core;

namespace Services.DTOs.MaintenanceDTO
{
    public class CreateMaintenanceDTO
    {
        public int MaintenanceId { get; set; }

        public string MaintenanceName { get; set; } = null!;

        public DateOnly MaintanaceDate { get; set; }

        public string? Description { get; set; }

        public string CreatedBy { get; set; } = null!;

        public string Department { get; set; } = null!;

        public List<string> EquipmentId { get; set; } = [];

        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(MaintenanceName) || string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(CreatedBy)
                || string.IsNullOrWhiteSpace(MaintanaceDate.ToString()) || string.IsNullOrWhiteSpace(Department))
            {
                throw new BusinessException("Vui lòng điền đầy đủ thông tin");
            }
            if(EquipmentId.Count <= 0)
            {
                throw new BusinessException("Danh sách thiết bị trống! Vui lòng chọn ít nhất 1 thiết bị");
            }
        }
    }
}
