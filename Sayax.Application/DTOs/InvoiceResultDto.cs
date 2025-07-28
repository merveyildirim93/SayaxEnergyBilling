using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Application.DTOs
{
    public class InvoiceResultDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal EnergyCost { get; set; }
        public decimal DistributionCost { get; set; }
        public decimal Btv { get; set; }
        public decimal Kdv { get; set; }
        public decimal TotalInvoice { get; set; }
    }
}
