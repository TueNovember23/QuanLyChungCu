using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Base;
using Repositories.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class RegulationRepository : IRegulationRepository
    {
        private readonly ApplicationDbContext _context;

        public RegulationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Regulation>> GetRegulaAllAsync()
        {
            return await _context.Regulations
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<Regulation?> GetRegulaByIdAsync(int id)
        {
            return await _context.Regulations
                .FirstOrDefaultAsync(r => r.RegulationId == id);
        }

        public async Task<IEnumerable<Regulation>> SearchRegulaAsync(string searchText)
        {
            return await _context.Regulations
                .Where(r => r.Title.Contains(searchText) ||
                           r.Content.Contains(searchText) ||
                           r.Category.Contains(searchText))
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task AddRegulaAsync(Regulation regulation)
        {
            await _context.Regulations.AddAsync(regulation);
        }

        public void UpdateRegula(Regulation regulation)
        {
            _context.Regulations.Update(regulation);
        }

        public async Task DeleteRegulaAsync(int id)
        {
            var regulation = await GetRegulaByIdAsync(id);
            if (regulation != null)
            {
                regulation.IsActive = false;
                UpdateRegula(regulation);
            }
        }
    }
}
