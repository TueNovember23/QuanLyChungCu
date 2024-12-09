using Services.DTOs.PaymentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.AccountantServices
{
    public interface IPaymentService
    {
        Task<List<ResponsePaymentDTO>> GetPayments(int month, int year, string status);
        Task<List<ResponsePaymentDTO>> SearchPayments(string searchText, int month, int year, string status);
       
        Task<List<PaymentHistoryDTO>> GetPaymentHistory(int invoiceId);
        Task GenerateReceipt(int invoiceId);
    }
}
