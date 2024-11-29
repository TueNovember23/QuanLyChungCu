using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.InvoiceDTO
{
    public class VehicleDetailDTO
    {
        public string VehicleId { get; set; } = null!;
        public double Price { get; set; }
    }
}
