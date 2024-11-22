using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.ApartmentDTO
{
    public class ApartmentDTO
    {
        public int ApartmentId { get; set; }
        public string ApartmentCode { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int FloorNumber { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        
    }
}