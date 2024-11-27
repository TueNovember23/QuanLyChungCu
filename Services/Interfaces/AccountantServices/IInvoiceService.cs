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
        Task<List<ResponseInvoiceDTO>> GetAll();
        Task<List<ResponseInvoiceDTO>> Search(string searchText);
        Task GenerateInvoices(int month, int year);
        Task SendInvoices(int month, int year);
    }
}
