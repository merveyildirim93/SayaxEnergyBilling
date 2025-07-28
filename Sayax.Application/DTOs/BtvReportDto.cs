using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Application.DTOs
{
    public class BtvReportDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal BtvAmount { get; set; }
        public string Municipality { get; set; }
    }
}
