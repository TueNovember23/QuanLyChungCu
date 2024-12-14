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
        Task<List<ResponseInvoiceDTO>> GetAllInvoices(int month, int year);
        Task GenerateInvoices(int month, int year, List<InvoiceInputDTO> invoiceInputs);
        Task GenerateWaterInvoices(int invoiceId, Apartment apartment, int startIndex, int endIndex, int numberOfPeople);
        Task GenerateManagementFeeInvoices(int invoiceId, Apartment apartment, double apartmentArea, double price);
        Task GenerateVehicleInvoices(int invoiceId, Apartment apartment);
        Task<List<Apartment>> GetApartmentsAsync();
        Task<List<ResponseWaterInvoiceDTO>> GetWaterInvoices(int month, int year);
        Task<List<ResponseVehicleInvoiceDTO>> GetVehicleInvoices(int month, int year);
        Task<List<ResponseManagementFeeInvoiceDTO>> GetManagementFeeInvoices(int month, int year);
        Task UpdateInvoiceStatus(int invoiceId, string status);
        Task <VechicleInvoice> GetVehicleInvoiceById(int id);
    }
}
