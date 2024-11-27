using Repositories.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IViolationRepository
    {
        Task<IEnumerable<Violation>> GetViolatAllAsync();
        Task<Violation?> GetViolatByIdAsync(int id);
        Task<IEnumerable<Violation>> SearchViolatAsync(string searchText);
        Task AddViolatAsync(Violation violation);
        void UpdateViolat(Violation violation);
        Task DeleteViolatAsync(int id);
    }
}
