using Castle.Components.DictionaryAdapter;
using Core;
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


        public async Task<List<ResponseInvoiceDTO>> GetAllInvoices(int month, int year)
        {
            var list = await _unitOfWork.GetRepository<Invoice>().Entities
                .Where(_ => _.Month == month && _.Year == year).ToListAsync();
            var listI = list.Select(_ => new ResponseInvoiceDTO()
            {
                InvoiceId = _.InvoiceId,
                ApartmentCode = _.WaterInvoices.FirstOrDefault()?.Apartment?.ApartmentCode,
                Month = _.Month,
                Year = _.Year,
                TotalAmount = _.TotalAmount,
                CreatedDate = _.CreatedDate,
                Status = _.Status,
                CreatedBy = _.CreatedByNavigation.FullName
            }).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var waterInvoice = _unitOfWork.GetRepository<WaterInvoice>().Entities.Where(_ => _.InvoiceId == list[i].InvoiceId).FirstOrDefault();
                if (waterInvoice != null)
                {
                    listI[i].ApartmentCode = waterInvoice.Apartment.ApartmentCode!;
                    continue;
                }
                var managementInvoice = _unitOfWork.GetRepository<ManagementFeeInvoice>().Entities.Where(_ => _.InvoiceId == list[i].InvoiceId).FirstOrDefault();
                if (managementInvoice != null)
                {
                    listI[i].ApartmentCode = managementInvoice.Apartment.ApartmentCode;
                    continue;
                }
            }
            return listI;
        }

        public async Task<List<ResponseWaterInvoiceDTO>> GetWaterInvoices(int month, int year)
        {
            var waterInvoices = await _unitOfWork.GetRepository<WaterInvoice>().Entities
                .Include(w => w.Apartment)
                .Where(w => w.Invoice.Month == month && w.Invoice.Year == year)
                .Select(w => new ResponseWaterInvoiceDTO
                {
                    WaterInvoiceId = w.WaterInvoiceId,
                    ApartmentCode = w.Apartment.ApartmentCode,
                    StartIndex = w.StartIndex,
                    EndIndex = w.EndIndex,
                    NumberOfPeople = w.NumberOfPeople,
                    TotalAmount = w.TotalAmount ?? 0,
                    CreatedDate = w.Invoice.CreatedDate ?? new(),
                    InvoiceId = w.InvoiceId
                })
                .ToListAsync();

            return waterInvoices;
        }

        public async Task<List<ResponseVehicleInvoiceDTO>> GetVehicleInvoices(int month, int year)
        {
            var vehicleInvoices = await _unitOfWork.GetRepository<VechicleInvoice>().Entities
                .Include(v => v.Apartment)
                .Where(v => v.Invoice.Month == month && v.Invoice.Year == year)
                .Select(v => new ResponseVehicleInvoiceDTO
                {
                    VehicleInvoiceId = v.VechicleInvoiceId,
                    ApartmentCode = v.Apartment.ApartmentCode,
                    TotalAmount = v.TotalAmount ?? 0,
                    CreatedDate = v.Invoice.CreatedDate ?? new(),
                    InvoiceId = v.InvoiceId
                })
                .ToListAsync();

            return vehicleInvoices;
        }

        public async Task<List<ResponseManagementFeeInvoiceDTO>> GetManagementFeeInvoices(int month, int year)
        {
            var managementInvoices = await _unitOfWork.GetRepository<ManagementFeeInvoice>().Entities
                .Include(m => m.Apartment)
                .Where(m => m.Invoice.Month == month && m.Invoice.Year == year)
                .Select(m => new ResponseManagementFeeInvoiceDTO
                {
                    ManagementFeeInvoiceId = m.ManagementFeeInvoiceId,
                    ApartmentCode = m.Apartment.ApartmentCode,
                    Price = m.Price,
                    TotalAmount = m.TotalAmount ?? 0,
                    CreatedDate = m.Invoice.CreatedDate ?? new(),
                    InvoiceId = m.InvoiceId,
                    Fee = m.ManagementFeeHistory.Price,
                    Area = m.Apartment.Area
                })
                .ToListAsync();

            return managementInvoices;
        }

        public async Task<VechicleInvoice> GetVehicleInvoiceById(int id)
        {
            var invoice = await _unitOfWork.GetRepository<VechicleInvoice>().Entities
                .FirstOrDefaultAsync(_ => _.InvoiceId == id) ?? throw new BusinessException("Không tìm thấy hóa đơn");
            return invoice;
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

        public async Task<List<ResponseVehicle>> GetDetailVehicleInvoiceById(int id)
        {
            List<ResponseVehicle> vehicles = await _unitOfWork.GetRepository<VechicleInvoiceDetail>().Entities.Where(_ => _.VechicleInvoiceId == id)
                .Select(_ => new ResponseVehicle() { Id = _.VehicleId, Fee = _.Vehicle.VehicleCategory.MonthlyFee, Type = _.Vehicle.VehicleCategory.CategoryName, Owner = _.Vehicle.VehicleOwner }).ToListAsync();
            return vehicles;
        }

        public async Task<Apartment> GetApartmentByCode(string apartmentCode)
        {
            var apartment = await _unitOfWork.GetRepository<Apartment>().Entities.FirstOrDefaultAsync(_ => _.ApartmentCode == apartmentCode)
                ?? throw new BusinessException($"Không tìm thấy căn hộ có mã {apartmentCode}");
            return apartment;
        }

        public async Task<int> GetLastWaterInvoiceStartIndex(string apartmentCode)
        {
            var lastInvoice = await _unitOfWork
                .GetRepository<WaterInvoice>()
                .Entities
                .Where(_ => _.Apartment.ApartmentCode == apartmentCode)
                .OrderByDescending(_ => _.Invoice.CreatedDate)
                .FirstOrDefaultAsync();

            return lastInvoice?.StartIndex ?? 0;
        }
        public async Task<ManagementFee> GetCurrentManagementFee()
        {
            ManagementFee fee = await _unitOfWork.GetRepository<ManagementFee>().Entities.FirstOrDefaultAsync(_ => _.DeletedDate == null)
                ?? throw new BusinessException("Bảng giá phí quản lý bị lỗi");
            return fee;
        }

        public async Task<List<ResponseVehicle>> GetVehiclesByApartmentCode(string apartmentCode)
        {
            List<ResponseVehicle> vehicles = await _unitOfWork.GetRepository<Vehicle>().Entities.Where(_ => _.Status == "Đang gửi" && _.Apartment.ApartmentCode == apartmentCode)
                .Select(_ => new ResponseVehicle() {Id = _.VehicleId, Owner = _.VehicleOwner, Fee = _.VehicleCategory.MonthlyFee, Type = _.VehicleCategory.CategoryName }).ToListAsync();
            return vehicles;
        }

        public async Task<WaterFee> GetCurrentWaterFee()
        {
            WaterFee fee = await _unitOfWork.GetRepository<WaterFee>().Entities.FirstOrDefaultAsync(_ => _.DeletedDate == null)
                ?? throw new BusinessException("Bảng giá nước bị lỗi");
            return fee;
        }

        public async Task<Account> GetAccountByUsername(string username)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(_ => _.Username == username)
                                ?? throw new BusinessException($"Không tìm thấy tài khoản có username {username}");
            return account;
        }

        public async Task CreateInvoice(Invoice invoice, WaterInvoice waterInvoice, ManagementFeeInvoice managementFeeInvoice, VechicleInvoice vechicleInvoice, List<VechicleInvoiceDetail> vechicleInvoiceDetails)
        {
            bool isExisted = _unitOfWork.GetRepository<Invoice>().Entities.Any(_ =>
                _.Month == invoice.Month
                && _.Year == invoice.Year
                && (_.WaterInvoices.Any(w => w.Apartment.ApartmentId == waterInvoice.ApartmentId)
                    || _.ManagementFeeInvoices.Any(m => m.Apartment.ApartmentId == managementFeeInvoice.ApartmentId)
                    || _.VechicleInvoices.Any(v => v.Apartment.ApartmentId == vechicleInvoice.ApartmentId))
            );
            if (isExisted)
            {
                throw new BusinessException($"Hóa đơn căn hộ {waterInvoice.Apartment.ApartmentCode} tháng {invoice.Month} năm {invoice.Year} đã được lập");
            }
            await _unitOfWork.GetRepository<Invoice>().InsertAsync(invoice);
            await _unitOfWork.SaveAsync();
            int id = invoice.InvoiceId;
            waterInvoice.InvoiceId = id;
            managementFeeInvoice.InvoiceId = id;
            vechicleInvoice.InvoiceId = id;
            await _unitOfWork.GetRepository<WaterInvoice>().InsertAsync(waterInvoice);
            await _unitOfWork.GetRepository<ManagementFeeInvoice>().InsertAsync(managementFeeInvoice);
            if (vechicleInvoiceDetails.Count > 0)
            {
                await _unitOfWork.GetRepository<VechicleInvoice>().InsertAsync(vechicleInvoice);
                await _unitOfWork.SaveAsync();
                foreach (var v in vechicleInvoiceDetails)
                {
                    v.VechicleInvoiceId = vechicleInvoice.VechicleInvoiceId;
                    await _unitOfWork.GetRepository<VechicleInvoiceDetail>().InsertAsync(v);
                }
            }
            await _unitOfWork.SaveAsync();
        }
    }

    public class ResponseVehicle
    {
        public string? Id { get; set; }
        public string? Owner { get; set; }
        public double? Fee { get; set; }
        public string? Type { get; set; }
    }
}
