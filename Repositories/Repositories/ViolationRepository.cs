﻿using Microsoft.EntityFrameworkCore;
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
                .Include(v => v.ViolationPenalties)
                .OrderByDescending(v => v.CreatedDate)
                .ToListAsync();
        }

        public async Task<Violation?> GetViolatByIdAsync(int id)
        {
            return await _context.Violations
                .Include(v => v.Apartment)
                .Include(v => v.Regulation)
                .Include(v => v.ViolationPenalties)
                .FirstOrDefaultAsync(v => v.ViolationId == id);
        }

        public async Task<IEnumerable<Violation>> SearchViolatAsync(string searchText)
        {
            using var context = new ApplicationDbContext();
            
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetViolatAllAsync();

            var normalizedSearch = searchText.Trim().ToLower();
            
            return await context.Violations
                .Include(v => v.Apartment)
                .Include(v => v.Regulation)
                .Include(v => v.ViolationPenalties)
                .Where(v =>
                    v.Apartment.ApartmentCode.ToLower().Contains(normalizedSearch) ||
                    v.Regulation.Title.ToLower().Contains(normalizedSearch) ||
                    (v.Detail != null && v.Detail.ToLower().Contains(normalizedSearch)) ||
                    v.ViolationPenalties.Any(p => 
                        (p.PenaltyMethod != null && p.PenaltyMethod.ToLower().Contains(normalizedSearch)) ||
                        (p.ProcessingStatus != null && p.ProcessingStatus.ToLower().Contains(normalizedSearch))
                    )
                )
                .OrderByDescending(v => v.CreatedDate)
                .AsNoTracking() 
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

        public async Task<IEnumerable<ViolationPenalty>> GetPenaltiesByViolationIdAsync(int violationId)
        {
            return await _context.ViolationPenalties
                .Where(p => p.ViolationId == violationId)
                .OrderByDescending(p => p.ProcessedDate)
                .ToListAsync();
        }

        public async Task AddPenaltyAsync(ViolationPenalty penalty)
        {
            await _context.ViolationPenalties.AddAsync(penalty);
        }

        public async Task UpdatePenaltyAsync(ViolationPenalty penalty)
        {
            _context.ViolationPenalties.Update(penalty);
        }

        public async Task<ViolationPenalty?> GetPenaltyByIdAsync(int penaltyId)
        {
            return await _context.ViolationPenalties
                .FirstOrDefaultAsync(p => p.PenaltyId == penaltyId);
        }

        public async Task<IEnumerable<Violation>> GetViolationsByRegulationId(int regulationId)
        {
            return await _context.Violations
                .Include(v => v.ViolationPenalties)
                .Where(v => v.RegulationId == regulationId)
                .ToListAsync();
        }
    }
}
