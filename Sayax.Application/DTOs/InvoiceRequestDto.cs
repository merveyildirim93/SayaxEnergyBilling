using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Application.DTOs
{
    public class InvoiceRequestDto
    {
        public int CustomerId { get; set; }
        public DateTime Month { get; set; }
    }
}
