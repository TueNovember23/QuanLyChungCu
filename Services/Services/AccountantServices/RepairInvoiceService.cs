using Repositories.Interfaces;
using Repositories.Repositories.Entities;
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

        public async Task<IEnumerable<RepairInvoice>> GetAllRepairInvoicesAsync()
        {
            return await _unitOfWork.GetRepository<RepairInvoice>().GetAllAsync();
        }

        public async Task<RepairInvoice?> GetRepairInvoiceByIdAsync(int id)
        {
            return await _unitOfWork.GetRepository<RepairInvoice>().GetByIdAsync(id);
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
