﻿using Repositories.Repositories.Entities;
using Services.DTOs.RepairInvoiceDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AccountantServices
{
    public interface IRepairInvoiceService
    {
        Task<List<ResponseRepairInvoiceDTO>> GetAllRepairInvoicesAsync();
        Task<List<ResponseRepairInvoiceDTO>> SearchRepairInvoicesAsync(string searchText);
        Task<ResponseRepairInvoiceDTO?> GetRepairInvoiceByIdAsync(int id);
        Task AddRepairInvoiceAsync(RepairInvoice invoice);
        Task DeleteRepairInvoiceAsync(int id);
    }
}