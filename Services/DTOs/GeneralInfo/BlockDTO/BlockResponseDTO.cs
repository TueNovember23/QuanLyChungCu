using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.GeneralInfo.BlockDTO
{
    public class BlockResponseDTO
    {
        public int BlockId { get; set; }
        public string BlockCode { get; set; }
        public int NumberOfFloor { get; set; }
        public int AreaId { get; set; }
    }
}
