using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.RepairInvoiceDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountantServices
{
    public class RepairInvoiceService : IRepairInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RepairInvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseRepairInvoiceDTO>> GetAllRepairInvoicesAsync()
        {
            var invoices = await _unitOfWork.GetRepository<RepairInvoice>().Entities
                .Select(invoice => new ResponseRepairInvoiceDTO
                {
                    InvoiceId = invoice.InvoiceId,
                    InvoiceContent = invoice.InvoiceContent,
                    TotalAmount = (decimal)invoice.TotalAmount,
                    Status = invoice.Status,
                    InvoiceDate = invoice.InvoiceDate.ToDateTime(new TimeOnly(0, 0))
                }).ToListAsync();

            return invoices;
        }

        public async Task<List<ResponseRepairInvoiceDTO>> SearchRepairInvoicesAsync(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return await GetAllRepairInvoicesAsync();
            }

            var query = _unitOfWork.GetRepository<RepairInvoice>().Entities
                .Where(invoice =>
                    invoice.InvoiceId.ToString().Contains(searchText) ||
                    (invoice.InvoiceContent != null && invoice.InvoiceContent.Contains(searchText)) ||
                    invoice.Status.Contains(searchText)
                );

            var response = await query.Select(invoice => new ResponseRepairInvoiceDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceContent = invoice.InvoiceContent,
                TotalAmount = (decimal)invoice.TotalAmount,
                Status = invoice.Status,
                InvoiceDate = invoice.InvoiceDate.ToDateTime(new TimeOnly(0, 0))
            }).ToListAsync();

            return response;
        }

        public async Task<ResponseRepairInvoiceDTO?> GetRepairInvoiceByIdAsync(int id)
        {
            var invoice = await _unitOfWork.GetRepository<RepairInvoice>().GetByIdAsync(id);
            if (invoice == null) return null;

            return new ResponseRepairInvoiceDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceContent = invoice.InvoiceContent,
                TotalAmount = (decimal)invoice.TotalAmount,
                Status = invoice.Status,
                InvoiceDate = invoice.InvoiceDate.ToDateTime(new TimeOnly(0, 0))
            };
        }

        public async Task AddRepairInvoiceAsync(RepairInvoice invoice)
        {
            await _unitOfWork.GetRepository<RepairInvoice>().InsertAsync(invoice);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteRepairInvoiceAsync(int id)
        {
            await _unitOfWork.GetRepository<RepairInvoice>().DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}
