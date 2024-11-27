using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.InvoiceDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountantServices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseInvoiceDTO>> GetAll()
        {
            var invoices = await _unitOfWork.GetRepository<Invoice>().Entities
                .Include(i => i.WaterInvoices)
                .ThenInclude(wi => wi.Apartment)
                .Include(i => i.VechicleInvoices)
                .Select(invoice => new ResponseInvoiceDTO
                {
                    InvoiceId = invoice.InvoiceId,
                    Month = invoice.Month ?? 0,
                    Year = invoice.Year ?? 0,
                    TotalAmount = invoice.TotalAmount ?? 0,
                    Status = invoice.Status ?? "Pending",
                    // You'll need to calculate these from related entities
                    WaterFee = invoice.WaterInvoices.Sum(w => w.TotalAmount ?? 0),
                    ParkingFee = invoice.VechicleInvoices.Sum(v => v.TotalAmount ?? 0)
                })
                .ToListAsync();

            return invoices;
        }

        public async Task<List<ResponseInvoiceDTO>> Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await GetAll();
            }

            var query = _unitOfWork.GetRepository<Invoice>().Entities
                .Include(i => i.WaterInvoices)
                .ThenInclude(wi => wi.Apartment)
                .Include(i => i.VechicleInvoices)
                .Where(invoice =>
                    invoice.Month.ToString().Contains(searchText) ||
                    invoice.Year.ToString().Contains(searchText) ||
                    invoice.Status!.Contains(searchText) ||
                    invoice.WaterInvoices.Any(w => w.Apartment.ApartmentCode.Contains(searchText))
                );

            var response = await query
                .Select(invoice => new ResponseInvoiceDTO
                {
                    InvoiceId = invoice.InvoiceId,
                    Month = invoice.Month ?? 0,
                    Year = invoice.Year ?? 0,
                    TotalAmount = invoice.TotalAmount ?? 0,
                    Status = invoice.Status ?? "Pending",
                    WaterFee = invoice.WaterInvoices.Sum(w => w.TotalAmount ?? 0),
                    ParkingFee = invoice.VechicleInvoices.Sum(v => v.TotalAmount ?? 0)
                })
                .ToListAsync();

            return response;
        }

        public async Task GenerateInvoices(int month, int year)
        {
            // Implementation for generating invoices
            // This would create new Invoice records and associated WaterInvoice, VechicleInvoice etc.
            throw new NotImplementedException();
        }

        public async Task SendInvoices(int month, int year)
        {
            // Implementation for sending invoices
            // This would update the status of invoices and potentially trigger email notifications
            throw new NotImplementedException();
        }
    }
}
