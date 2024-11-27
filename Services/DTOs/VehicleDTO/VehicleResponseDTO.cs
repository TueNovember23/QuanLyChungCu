using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.VehicleDTO
{
    public class VehicleResponseDTO
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string VehicleOwner { get; set; } = string.Empty;
        public int ApartmentId { get; set; }
        public string Status { get; set; } = null!;
        public float MonthlyFee { get; set; }
        public string ApartmentCode { get; set; } = null!;

        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public float PaymentAmount { get; set; }
    }
}
