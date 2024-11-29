using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.RegulationDTO
{
    public class RegulationResponseDTO
    {
        public int RegulationId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string Category { get; set; } = null!;
        public string Priority { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
