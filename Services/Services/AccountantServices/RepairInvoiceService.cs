using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories.Entities;
using Services.DTOs.EquipmentDTO;
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

        public async Task<List<ResponseEquipmentDTO>> GetAvailableEquipmentsAsync()
        {
            var equipments = await _unitOfWork.GetRepository<Equipment>().Entities
                .Where(e => e.Status == "Hỏng" && !e.IsDeleted)
                .Select(e => new ResponseEquipmentDTO
                {
                    EquipmentId = e.EquipmentId,
                    EquipmentName = e.EquipmentName,
                }).ToListAsync();

            return equipments;
        }



        public async Task<int> GenerateNewRepairInvoiceCodeAsync()
        {
            try
            {
                var totalInvoices = await _unitOfWork.GetRepository<RepairInvoice>().Entities.CountAsync();
                return totalInvoices + 1; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating new invoice code: " + ex.Message);
            }
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
                InvoiceDate = invoice.InvoiceDate.ToDateTime(new TimeOnly(0, 0)),
                MalfuntionEquipments = invoice.MalfuntionEquipments.Select(me => new MalfuntionEquipmentDTO
                {
                    EquipmentId = me.Equipment.EquipmentId,
                    EquipmentName = me.Equipment.EquipmentName,
                    RepairPrice = me.RepairPrice
                }).ToList()
            };
        }


        public async Task<MalfuntionEquipmentDTO?> GetMalfunctionEquipmentByIdAsync(int equipmentId)
        {
            var malfunctionEquipment = await _unitOfWork.GetRepository<MalfuntionEquipment>().Entities
                .Include(me => me.Equipment) 
                .Where(me => me.EquipmentId == equipmentId)
                .Select(me => new MalfuntionEquipmentDTO
                {
                    EquipmentId = me.EquipmentId,
                    EquipmentName = me.Equipment != null ? me.Equipment.EquipmentName : null,
                    RepairPrice = me.RepairPrice,
                  
                })
                .FirstOrDefaultAsync();
            return malfunctionEquipment;
        }


        public async Task AddRepairInvoiceAsync(CreateRepairInvoiceDTO invoiceDto)
        {
            var existingInvoice = await _unitOfWork.GetRepository<RepairInvoice>()
                .GetByIdAsync(invoiceDto.InvoiceId);

            if (existingInvoice != null)
            {
                existingInvoice.InvoiceContent = invoiceDto.InvoiceContent;
                existingInvoice.TotalAmount = invoiceDto.TotalAmount;
                existingInvoice.MalfuntionEquipments = invoiceDto.MalfunctionEquipments.Select(me => new MalfuntionEquipment
                {
                    EquipmentId = me.EquipmentId,
                    Description = me.Description,
                    SolvingMethod = me.SolvingMethod,
                    RepairPrice = me.RepairPrice
                }).ToList();

                _unitOfWork.GetRepository<RepairInvoice>().Update(existingInvoice);
            }
            else
            {
                var invoice = new RepairInvoice
                {
                    InvoiceId = invoiceDto.InvoiceId,
                    InvoiceContent = invoiceDto.InvoiceContent,
                    TotalAmount = invoiceDto.TotalAmount,
                    //CreatedBy = invoiceDto.CreatedBy,
                    MalfuntionEquipments = invoiceDto.MalfunctionEquipments.Select(me => new MalfuntionEquipment
                    {
                        EquipmentId = me.EquipmentId,
                        Description = me.Description,
                        SolvingMethod = me.SolvingMethod,
                        RepairPrice = me.RepairPrice
                    }).ToList()
                };

                await _unitOfWork.GetRepository<RepairInvoice>().InsertAsync(invoice);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task AddRepairInvoiceWithDetailsAsync(RepairInvoice invoice, List<MalfunctionEquipmentDTO> malfunctionEquipments)
        {
            if (invoice == null || malfunctionEquipments == null || !malfunctionEquipments.Any())
            {
                throw new ArgumentException("Invoice and malfunction equipment list must not be null or empty.");
            }

            await _unitOfWork.GetRepository<RepairInvoice>().InsertAsync(invoice);

            foreach (var malfunction in malfunctionEquipments)
            {
                malfunction.RepairInvoiceId = invoice.InvoiceId;
                await _unitOfWork.GetRepository<MalfunctionEquipmentDTO>().InsertAsync(malfunction);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteRepairInvoiceAsync(int id)
        {
            await _unitOfWork.GetRepository<RepairInvoice>().DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        

    }
}
