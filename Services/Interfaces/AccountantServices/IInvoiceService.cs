using Repositories.Repositories.Entities;
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
        Task GenerateInvoices(int month, int year, List<InvoiceInputDTO> invoiceInputs);
        Task GenerateWaterInvoices(int invoiceId, Apartment apartment, int startIndex, int endIndex, int numberOfPeople);
        Task GenerateManagementFeeInvoices(int invoiceId, Apartment apartment, double apartmentArea, double price);
        Task GenerateVehicleInvoices(int invoiceId, Apartment apartment);
        Task<List<Apartment>> GetApartmentsAsync();
        Task UpdateInvoiceStatus(int invoiceId, string status);
    }
}
