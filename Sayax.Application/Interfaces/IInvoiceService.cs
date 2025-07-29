using Sayax.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResultDto> CalculateInvoiceAsync(InvoiceRequestDto request);
        Task<List<InvoiceResultDto>> CalculateInvoicesForAllCustomersAsync(DateTime month);
    }
}
