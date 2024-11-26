using Repositories.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AccountantServices
{
    public interface IRepairInvoiceService
    {
        Task<IEnumerable<RepairInvoice>> GetAllRepairInvoicesAsync();
        Task<RepairInvoice?> GetRepairInvoiceByIdAsync(int id);
        Task AddRepairInvoiceAsync(RepairInvoice invoice);
        Task DeleteRepairInvoiceAsync(int id);
    }
}
