using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.PaymentDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountantServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task GenerateReceipt(int invoiceId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PaymentHistoryDTO>> GetPaymentHistory(int invoiceId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ResponsePaymentDTO>> GetPayments(int month, int year, string status)
        {
            var query = _unitOfWork.GetRepository<Invoice>().Entities
                .Include(i => i.WaterInvoices).ThenInclude(wi => wi.Apartment)
                .Include(i => i.ManagementFeeInvoices)
                .Include(i => i.VechicleInvoices)

                .Where(i => i.Month == month && i.Year == year).ToList();

            if (status != "Tất cả")
            {
                query = query.Where(i => i.Status == status).ToList();
            }


            return query.Select(invoice => new ResponsePaymentDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceCode = $"INV{invoice.InvoiceId:D6}",
                ApartmentCode = invoice.WaterInvoices.FirstOrDefault()!.Apartment.ApartmentCode!,
                TotalAmount = (decimal)(invoice.TotalAmount ?? 0),
                PaidAmount = 0,
                RemainingAmount = (decimal)(invoice.TotalAmount ?? 0),
                DueDate = invoice.CreatedDate.HasValue
                ? invoice.CreatedDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(15)
                : DateTime.MinValue,
                Status = invoice.Status
            }).ToList();
        }

        public Task ProcessPayment(ProcessPaymentDTO payment)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ResponsePaymentDTO>> SearchPayments(string searchText, int month, int year, string status)
        {
            var query = _unitOfWork.GetRepository<Invoice>().Entities
                .Include(i => i.WaterInvoices).ThenInclude(wi => wi.Apartment)
                .Include(i => i.ManagementFeeInvoices)
                .Include(i => i.VechicleInvoices)
                .Where(i =>
                    i.Month == month &&
                    i.Year == year &&
                    (i.WaterInvoices.Any(w => w.Apartment.ApartmentCode!.Contains(searchText)) ||
                     i.Status.Contains(searchText))).ToList();

            if (status != "Tất cả")
            {
                query = query.Where(i => i.Status == status).ToList();
            }

            return query.Select(invoice => new ResponsePaymentDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceCode = $"INV{invoice.InvoiceId:D6}",
                ApartmentCode = invoice.WaterInvoices.FirstOrDefault()!.Apartment.ApartmentCode!,
                TotalAmount = (decimal)(invoice.TotalAmount ?? 0),
                PaidAmount = 0,
                RemainingAmount = (decimal)(invoice.TotalAmount ?? 0),
                DueDate = invoice.CreatedDate.HasValue
                ? invoice.CreatedDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(15)
                : DateTime.MinValue,
                Status = invoice.Status
            }).ToList();
        }

        // Implement other interface methods...
    }
}
