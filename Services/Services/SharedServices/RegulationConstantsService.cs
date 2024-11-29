using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.SharedServices
{
    public static class RegulationConstantsService
    {
        public static readonly List<string> Categories = new()
        {
            "An ninh",
            "Vệ sinh",
            "Phòng cháy",
            "Sinh hoạt",
            "Khác"
        };

        public static readonly List<string> PriorityLevels = new()
        {
            "Cao",
            "Trung bình",
            "Thấp"
        };
    }
}
