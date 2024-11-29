using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.DepartmentDTO
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public int NumberOfStaff { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}