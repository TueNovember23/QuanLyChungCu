using Repositories.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IRegulationRepository
    {
        Task<IEnumerable<Regulation>> GetRegulaAllAsync();
        Task<Regulation?> GetRegulaByIdAsync(int id);
        Task<IEnumerable<Regulation>> SearchRegulaAsync(string searchText);
        Task AddRegulaAsync(Regulation regulation);
        void UpdateRegula(Regulation regulation);
        Task DeleteRegulaAsync(int id);
    }
}
