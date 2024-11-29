using Repositories.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetDepartAllAsync();
        Task<Department?> GetDepartByIdAsync(int id);
        Task<Department?> GetDepartByNameAsync(string name);
        Task<IEnumerable<Department>> SearchDepartAsync(string keyword);
        Task AddDepartAsync(Department department);
        void UpdateDepart(Department department);
        void DeleteDepart(Department department);
    }
}
