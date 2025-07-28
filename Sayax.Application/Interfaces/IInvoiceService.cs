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
        InvoiceResultDto CalculateInvoice(int customerId, DateTime period);
    }
}
