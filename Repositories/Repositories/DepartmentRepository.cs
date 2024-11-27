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
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetDepartAllAsync()
        {
            return await _context.Departments
                .Where(d => !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<Department?> GetDepartByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<Department?> GetDepartByNameAsync(string name)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentName.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Department>> SearchDepartAsync(string keyword)
        {
            return await _context.Departments
                .Where(d => !d.IsDeleted &&
                    (d.DepartmentName.Contains(keyword) ||
                     d.Description!.Contains(keyword)))
                .ToListAsync();
        }

        public async Task AddDepartAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }

        public void UpdateDepart(Department department)
        {
            _context.Departments.Update(department);
        }

        public void DeleteDepart(Department department)
        {
            department.IsDeleted = true;
            _context.Departments.Update(department);
        }
    }
}
