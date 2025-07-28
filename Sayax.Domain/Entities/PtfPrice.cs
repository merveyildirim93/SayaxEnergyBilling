using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Domain.Entities
{
    public class PtfPrice
    {
        public DateTime DateTime { get; set; }
        public decimal PricePerMWh { get; set; }
    }
}
