using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.RegulationDTO
{
    public class RegulationDTO
    {
        public int RegulationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;

        // UI properties
        public string PriorityIcon => Priority switch
        {
            "Cao" => "AlertCircle",
            "Trung bình" => "Alert",
            "Thấp" => "AlertOutline",
            _ => "Alert"
        };

        public string CategoryIcon => Category switch
        {
            "An ninh" => "Security",
            "Vệ sinh" => "Broom",
            "Phòng cháy" => "Fire",
            "Sinh hoạt" => "Home",
            _ => "Information"
        };

        public string PriorityColor => Priority switch
        {
            "Cao" => "#FF0000",
            "Trung bình" => "#FFA500",
            "Thấp" => "#008000",
            _ => "#000000"
        };
    }
}
