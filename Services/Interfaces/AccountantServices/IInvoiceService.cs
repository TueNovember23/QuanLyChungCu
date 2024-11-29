using Services.DTOs.InvoiceDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AccountantServices
{
    public interface IInvoiceService
    {
        Task<InvoiceGroupDTO> GetAllInvoices(int month, int year);
        Task GenerateInvoices(int month, int year);
    }
}
