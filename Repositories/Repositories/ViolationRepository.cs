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
    public class ViolationRepository : IViolationRepository
    {
        private readonly ApplicationDbContext _context;

        public ViolationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Violation>> GetViolatAllAsync()
        {
            return await _context.Violations
                .Include(v => v.Apartment)
                .Include(v => v.Regulation)
                .ToListAsync();
        }

        public async Task<Violation?> GetViolatByIdAsync(int id)
        {
            return await _context.Violations
                .Include(v => v.Apartment)
                .Include(v => v.Regulation)
                .FirstOrDefaultAsync(v => v.ViolationId == id);
        }

        public async Task<IEnumerable<Violation>> SearchViolatAsync(string searchText)
        {
            return await _context.Violations
                .Include(v => v.Apartment)
                .Include(v => v.Regulation)
                .Where(v =>
                    v.Apartment.ApartmentCode.Contains(searchText) ||
                    v.Regulation.Title.Contains(searchText) ||
                    v.Detail.Contains(searchText))
                .ToListAsync();
        }

        public async Task AddViolatAsync(Violation violation)
        {
            await _context.Violations.AddAsync(violation);
        }

        public void UpdateViolat(Violation violation)
        {
            _context.Violations.Update(violation);
        }

        public async Task DeleteViolatAsync(int id)
        {
            var violation = await _context.Violations.FindAsync(id);
            if (violation != null)
            {
                _context.Violations.Remove(violation);
            }
        }
    }
}
