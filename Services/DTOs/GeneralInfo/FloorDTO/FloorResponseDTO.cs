using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.GeneralInfo.FloorDTO
{
    public class FloorResponseDTO
    {
        public int FloorId { get; set; }
        public int FloorNumber { get; set; }
        public int BlockId { get; set; }
        public int NumberOfApartment { get; set; }
        public string Status => NumberOfApartment > 0 ? "Có căn hộ" : "Trống";
    }
}
