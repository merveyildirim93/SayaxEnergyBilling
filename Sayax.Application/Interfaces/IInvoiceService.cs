using Sayax.Application.DTOs;

namespace Sayax.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResultDto> CalculateInvoiceAsync(InvoiceRequestDto request);
        Task<List<InvoiceResultDto>> CalculateInvoicesForAllCustomersAsync(DateTime month);
    }
}
