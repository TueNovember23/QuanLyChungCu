using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.InvoiceDTO;
using Services.Interfaces.AccountantServices;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<InvoiceGroupDTO> GetAllInvoices(int month, int year)
        {
            var invoices = await _unitOfWork.GetRepository<Invoice>().Entities
                .Include(i => i.WaterInvoices)
                    .ThenInclude(w => w.Apartment)
                .Include(i => i.VechicleInvoices)
                    .ThenInclude(v => v.Apartment)
                //.Include(i => i.ManagementFeeInvoices)
                //    .ThenInclude(m => m.Apartment)
                .Where(i => i.Month == month && i.Year == year)
                .ToListAsync();

            return new InvoiceGroupDTO
            {
                TotalInvoices = invoices.Select(i => new ResponseInvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    Month = i.Month ?? 0,
                    Year = i.Year ?? 0,
                    TotalAmount = i.TotalAmount ?? 0,
                    Status = i.Status ?? "Pending",
                    CreatedDate = i.CreatedDate ?? DateOnly.FromDateTime(DateTime.Now)
                }).ToList(),

                WaterInvoices = invoices.SelectMany(i => i.WaterInvoices.Select(w => new ResponseWaterInvoiceDTO
                {
                    WaterInvoiceId = w.WaterInvoiceId,
                    ApartmentCode = w.Apartment.ApartmentCode,
                    StartIndex = w.StartIndex,
                    EndIndex = w.EndIndex,
                    NumberOfPeople = w.NumberOfPeople,
                    TotalAmount = w.TotalAmount ?? 0,
                    CreatedDate = i.CreatedDate ?? DateOnly.FromDateTime(DateTime.Now),
                    InvoiceId = i.InvoiceId
                })).ToList(),

                //ManagementInvoices = invoices.SelectMany(i => i.ManagementFeeInvoices.Select(m => new ResponseManagementFeeInvoiceDTO
                //{
                //    ManagementFeeInvoiceId = m.ManagementFeeInvoiceId,
                //    ApartmentCode = m.Apartment.ApartmentCode,
                //    Price = m.Price,
                //    TotalAmount = m.TotalAmount ?? 0,
                //    CreatedDate = i.CreatedDate ?? DateOnly.FromDateTime(DateTime.Now),
                //    InvoiceId = i.InvoiceId
                //})).ToList(),

                VehicleInvoices = invoices.SelectMany(i => i.VechicleInvoices.Select(v => new ResponseVehicleInvoiceDTO
                {
                    VehicleInvoiceId = v.VechicleInvoiceId,
                    ApartmentCode = v.Apartment.ApartmentCode,
                    TotalAmount = v.TotalAmount ?? 0,
                    CreatedDate = i.CreatedDate ?? DateOnly.FromDateTime(DateTime.Now),
                    InvoiceId = i.InvoiceId
                })).ToList()
            };
        }

        public async Task GenerateInvoices(int month, int year)
        {
            try
            {
                // Bắt đầu giao dịch
                _unitOfWork.BeginTransaction();

                // 1. Tạo hóa đơn chính
                var invoice = new Invoice
                {
                    Month = month,
                    Year = year,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    Status = "Pending"
                };
                await _unitOfWork.GetRepository<Invoice>().InsertAsync(invoice);
                await _unitOfWork.SaveAsync();

                // 2. Tạo hóa đơn nước
                await GenerateWaterInvoices(invoice.InvoiceId);

                // 3. Tạo hóa đơn phí quản lý
                await GenerateManagementFeeInvoices(invoice.InvoiceId);

                // 4. Tạo hóa đơn xe
                await GenerateVehicleInvoices(invoice.InvoiceId);

                // 5. Cập nhật tổng số tiền
                var updatedInvoice = await _unitOfWork.GetRepository<Invoice>().Entities
                    .Include(i => i.WaterInvoices)
                    .Include(i => i.VechicleInvoices)
                    //.Include(i => i.ManagementFeeInvoices)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);

                if (updatedInvoice != null)
                {
                    updatedInvoice.TotalAmount = (
                        updatedInvoice.WaterInvoices.Sum(w => w.TotalAmount ?? 0) +
                        updatedInvoice.VechicleInvoices.Sum(v => v.TotalAmount ?? 0) /*+*/
                    //updatedInvoice.ManagementFeeInvoices.Sum(m => m.TotalAmount ?? 0)
                    );
                    await _unitOfWork.SaveAsync();
                }

                // Commit giao dịch
                _unitOfWork.CommitTransaction();
            }
            catch
            {
                // Rollback giao dịch
                _unitOfWork.RollBack();
                throw;
            }
        }


        private async Task GenerateWaterInvoices(int invoiceId)
        {
            // Implementation for generating water invoices
            throw new NotImplementedException();
        }

        private async Task GenerateManagementFeeInvoices(int invoiceId)
        {
            // Implementation for generating management fee invoices
            throw new NotImplementedException();
        }

        private async Task GenerateVehicleInvoices(int invoiceId)
        {
            // Implementation for generating vehicle invoices
            throw new NotImplementedException();
        }
    }
}
