using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.PaymentDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<Apartment>> GetApartmentsAsync()
        {
            return await _unitOfWork.GetRepository<Apartment>().Entities.ToListAsync();
        }

        public async Task<List<ResponsePaymentDTO>> GetPayments(int month, int year, string status)
        {
            var query = _unitOfWork.GetRepository<Invoice>().Entities
                .Include(i => i.WaterInvoices).ThenInclude(wi => wi.Apartment)
                .Include(i => i.ManagementFeeInvoices)
                .Include(i => i.VechicleInvoices)
                .Where(i => i.Month == month && i.Year == year);

            if (status != "Tất cả")
            {
                query = query.Where(i => i.Status == status);
            }

            var invoices = await query.ToListAsync();

            return invoices.Select(invoice => new ResponsePaymentDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceCode = $"INV{invoice.InvoiceId:D6}",
                ApartmentCode = invoice.WaterInvoices.FirstOrDefault()?.Apartment?.ApartmentCode ?? "Không có",
                TotalAmount = (decimal)(invoice.TotalAmount ?? 0),
                PaidAmount = 0, 
                RemainingAmount = (decimal)(invoice.TotalAmount ?? 0),
                DueDate = invoice.CreatedDate?.ToDateTime(TimeOnly.MinValue).AddDays(15) ?? DateTime.MinValue,
                Status = invoice.Status ?? "Chưa thanh toán"
            }).ToList();
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
                    (i.WaterInvoices.Any(w => w.Apartment.ApartmentCode.Contains(searchText)) ||
                     i.Status.Contains(searchText)));

            if (status != "Tất cả")
            {
                query = query.Where(i => i.Status == status);
            }

            var invoices = await query.ToListAsync();

            return invoices.Select(invoice => new ResponsePaymentDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceCode = $"INV{invoice.InvoiceId:D6}",
                ApartmentCode = invoice.WaterInvoices.FirstOrDefault()?.Apartment?.ApartmentCode ?? "Không có",
                TotalAmount = (decimal)(invoice.TotalAmount ?? 0),
                PaidAmount = 0, 
                RemainingAmount = (decimal)(invoice.TotalAmount ?? 0),
                DueDate = invoice.CreatedDate?.ToDateTime(TimeOnly.MinValue).AddDays(15) ?? DateTime.MinValue,
                Status = invoice.Status ?? "Chưa thanh toán"
            }).ToList();
        }

        public async Task UpdatePaymentStatus(int invoiceId, decimal paidAmount, string type = "Invoice")
        {
            if (type == "Invoice")
            {
                var invoice = await _unitOfWork.GetRepository<Invoice>().GetByIdAsync(invoiceId);
                if (invoice != null)
                {
                    if (paidAmount >= (decimal)(invoice.TotalAmount ?? 0))
                    {
                        invoice.Status = "Đã thanh toán";
                    }
                    else if (paidAmount > 0)
                    {
                        invoice.Status = "Thanh toán một phần";
                    }
                    else
                    {
                        invoice.Status = "Chưa thanh toán";
                    }

                    await _unitOfWork.SaveAsync();
                }
            }
            else if (type == "RepairInvoice")
            {
                var invoice = await _unitOfWork.GetRepository<RepairInvoice>().GetByIdAsync(invoiceId);
                if (invoice != null)
                {
                    if (paidAmount >= (decimal)(invoice.TotalAmount))
                    {
                        invoice.Status = "Đã thanh toán";
                    }
                    else if (paidAmount > 0)
                    {
                        invoice.Status = "Thanh toán một phần";
                    }
                    else
                    {
                        invoice.Status = "Chưa thanh toán";
                    }

                    await _unitOfWork.SaveAsync();
                }
            }
        }
    }
}