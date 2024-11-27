using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.ViolationDTO
{
    public class ViolationResponseDTO
    {
        public int ViolationId { get; set; }
        public string ApartmentCode { get; set; } = null!;
        public string RegulationTitle { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? Detail { get; set; }
    }

}
