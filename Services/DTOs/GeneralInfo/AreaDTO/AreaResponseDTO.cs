using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.GeneralInfo.AreaDTO
{
    public class AreaResponseDTO
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string Location { get; set; }
        public int BlockCount { get; set; }
    }
}
