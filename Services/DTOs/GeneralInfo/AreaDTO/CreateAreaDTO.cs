using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.GeneralInfo.AreaDTO
{
    public class CreateAreaDTO
    {
        public string AreaName { get; set; }
        public string Location { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(AreaName)
                && !string.IsNullOrWhiteSpace(Location);
        }
    }
}
