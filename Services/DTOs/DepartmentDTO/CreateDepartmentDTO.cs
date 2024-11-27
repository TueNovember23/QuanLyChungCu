using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.DepartmentDTO
{
    public class CreateDepartmentDTO
    {
        public string DepartmentName { get; set; } = null!;
        public int NumberOfStaff { get; set; }
        public string? Description { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(DepartmentName))
                throw new BusinessException("Tên bộ phận không được để trống");

            if (NumberOfStaff < 0)
                throw new BusinessException("Số lượng nhân viên không hợp lệ");

            return true;
        }
    }
}
