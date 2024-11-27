using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.RegulationDTO
{
    public class CreateRegulationDTO
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Priority { get; set; } = null!;
        public bool IsActive { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Title))
                throw new BusinessException("Tiêu đề không được để trống");

            if (string.IsNullOrWhiteSpace(Content))
                throw new BusinessException("Nội dung không được để trống");

            if (string.IsNullOrWhiteSpace(Category))
                throw new BusinessException("Phân loại không được để trống");

            if (string.IsNullOrWhiteSpace(Priority))
                throw new BusinessException("Mức độ ưu tiên không được để trống");

            return true;
        }
    }
}
