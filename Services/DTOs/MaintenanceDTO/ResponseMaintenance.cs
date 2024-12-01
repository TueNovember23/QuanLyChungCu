namespace Services.DTOs.MaintenanceDTO
{
    public class ResponseMaintenance
    {
        public int MaintenanceId { get; set; }

        public string MaintenanceName { get; set; } = null!;

        public string MaintanaceDate { get; set; } = null!;

        public string? Description { get; set; }

        public string? Status { get; set; }

        public string CreatedBy { get; set; } = null!;

        public string Department { get; set; } = null!;
    }
}
