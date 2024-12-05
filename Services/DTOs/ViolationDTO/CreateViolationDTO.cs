using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.ViolationDTO
{
    public class CreateViolationDTO
    {
        public int ApartmentId { get; set; }
        public int RegulationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Detail { get; set; }

        public bool IsValid()
        {
            if (ApartmentId <= 0)
                throw new BusinessException("Vui lòng chọn căn hộ");

            if (RegulationId <= 0)
                throw new BusinessException("Vui lòng chọn nội quy");

            if (CreatedDate > DateTime.Today)
                throw new BusinessException("Ngày vi phạm không được ở tương lai");

            return true;
        }
    }
}
