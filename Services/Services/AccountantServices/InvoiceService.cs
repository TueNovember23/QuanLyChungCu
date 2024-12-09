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

        public async Task<List<Apartment>> GetApartmentsAsync()
        {
            return await _unitOfWork.GetRepository<Apartment>().Entities.ToListAsync();
        }


        public async Task<InvoiceGroupDTO> GetAllInvoices(int month, int year)
        {
            var invoices = await _unitOfWork.GetRepository<Invoice>().Entities
                .Include(i => i.WaterInvoices)
                    .ThenInclude(w => w.Apartment)
                .Include(i => i.VechicleInvoices)
                    .ThenInclude(v => v.Apartment)
                .Include(i => i.ManagementFeeInvoices)
                    .ThenInclude(m => m.Apartment)
                .Where(i => i.Month == month && i.Year == year)
                .ToListAsync();
            Console.WriteLine($"Số lượng hóa đơn xe: {invoices.SelectMany(i => i.VechicleInvoices).Count()}");
            return new InvoiceGroupDTO
            {
                TotalInvoices = invoices.Select(i => new ResponseInvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    ApartmentCode = i.VechicleInvoices.FirstOrDefault()?.Apartment.ApartmentCode,
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

                ManagementInvoices = invoices.SelectMany(i => i.ManagementFeeInvoices.Select(m => new ResponseManagementFeeInvoiceDTO
                {
                    ManagementFeeInvoiceId = m.ManagementFeeInvoiceId,
                    ApartmentCode = m.Apartment.ApartmentCode,
                    Price = m.Price,
                    TotalAmount = m.TotalAmount ?? 0,
                    CreatedDate = i.CreatedDate ?? DateOnly.FromDateTime(DateTime.Now),
                    InvoiceId = i.InvoiceId
                })).ToList(),

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

        public async Task GenerateInvoices(int month, int year, List<InvoiceInputDTO> invoiceInputs)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var apartments = await _unitOfWork.GetRepository<Apartment>().Entities.ToListAsync();

                foreach (var apartment in apartments)
                {
                    var existingInvoice = await _unitOfWork.GetRepository<ResponseInvoiceDTO>().Entities
                        .FirstOrDefaultAsync(i => i.Month == month && i.Year == year && i.ApartmentCode == apartment.ApartmentCode);

                    if (existingInvoice != null)
                    {
                        continue;
                    }

                    var invoice = new ResponseInvoiceDTO
                    {
                        ApartmentCode = apartment.ApartmentCode,
                        Month = month,
                        Year = year,
                        CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                        Status = "Pending"
                    };

                    await _unitOfWork.GetRepository<ResponseInvoiceDTO>().InsertAsync(invoice);
                    await _unitOfWork.SaveAsync();

                    var invoiceInput = invoiceInputs.FirstOrDefault(i => i.ApartmentCode == apartment.ApartmentCode);

                    if (invoiceInput != null)
                    {
                        await GenerateWaterInvoices(invoice.InvoiceId, apartment, invoiceInput.StartIndex, invoiceInput.EndIndex, invoiceInput.NumberOfPeople);
                        await GenerateManagementFeeInvoices(invoice.InvoiceId, apartment, invoiceInput.ApartmentArea, invoiceInput.Price);
                        await GenerateVehicleInvoices(invoice.InvoiceId, apartment);
                    }

                    invoice.TotalAmount = (
                        invoice.WaterInvoices.Sum(w => w.TotalAmount) +
                        invoice.ManagementFeeInvoices.Sum(m => m.TotalAmount) +
                        invoice.VechicleInvoices.Sum(v => v.TotalAmount)
                    );

                    await _unitOfWork.SaveAsync();
                }

                _unitOfWork.CommitTransaction();
            }
            catch
            {
                _unitOfWork.RollBack();
                throw;
            }
        }


        public async Task GenerateWaterInvoices(int invoiceId, Apartment apartment, int startIndex, int endIndex, int numberOfPeople)
        {
            var waterInvoice = new ResponseWaterInvoiceDTO
            {
                InvoiceId = invoiceId,
                ApartmentCode = apartment.ApartmentCode,
                StartIndex = startIndex,
                EndIndex = endIndex,
                NumberOfPeople = numberOfPeople,
                TotalAmount = (endIndex - startIndex) * 10, 
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _unitOfWork.GetRepository<ResponseWaterInvoiceDTO>().InsertAsync(waterInvoice);
        }

        public async Task GenerateManagementFeeInvoices(int invoiceId, Apartment apartment, double apartmentArea, double price)
        {
            var managementFeeInvoice = new ResponseManagementFeeInvoiceDTO
            {
                InvoiceId = invoiceId,
                ApartmentCode = apartment.ApartmentCode,
                Price = price,
                TotalAmount = price * apartmentArea,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _unitOfWork.GetRepository<ResponseManagementFeeInvoiceDTO>().InsertAsync(managementFeeInvoice);
        }

        public async Task GenerateVehicleInvoices(int invoiceId, Apartment apartment)
        {
            var vehicleInvoice = new ResponseVehicleInvoiceDTO
            {
                InvoiceId = invoiceId,
                ApartmentCode = apartment.ApartmentCode,
                TotalAmount = 200, 
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _unitOfWork.GetRepository<ResponseVehicleInvoiceDTO>().InsertAsync(vehicleInvoice);
        }

        public async Task UpdateInvoiceStatus(int invoiceId, string status)
        {
            var invoice = await _unitOfWork.GetRepository<Invoice>().FindAsync(i => i.InvoiceId == invoiceId);
            if (invoice != null)
            {
                invoice.Status = status;
                await _unitOfWork.SaveAsync();
            }
        }

    }
}
