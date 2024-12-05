using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.ViolationDTO
{
    public class ViolationPenaltyDTO
    {
        public int PenaltyId { get; set; }
        public int ViolationId { get; set; }
        public string PenaltyLevel { get; set; } = null!;
        public decimal Fine { get; set; }
        public string PenaltyMethod { get; set; } = null!;
        public string ProcessingStatus { get; set; } = "Chờ xử lý";
        public DateTime ProcessedDate { get; set; }
        public string? Note { get; set; }

        public bool IsValid()
        {
            if (ViolationId <= 0)
                throw new BusinessException("Vi phạm không hợp lệ");
            if (string.IsNullOrEmpty(PenaltyLevel))
                throw new BusinessException("Vui lòng chọn mức độ xử phạt");
            if (Fine <= 0)
                throw new BusinessException("Số tiền phạt phải lớn hơn 0");
            if (string.IsNullOrEmpty(PenaltyMethod))
                throw new BusinessException("Vui lòng nhập phương án xử lý");
            if (string.IsNullOrEmpty(ProcessingStatus))
                throw new BusinessException("Vui lòng chọn trạng thái xử lý");
            return true;
        }
    }
}
