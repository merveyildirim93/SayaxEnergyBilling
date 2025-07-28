using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Domain.Entities
{
    public class HourlyConsumption
    {
        public string MeterId { get; set; }
        public DateTime Date { get; set; }
        public int Hour { get; set; } // 1–24
        public decimal ConsumptionMWh { get; set; } // Örn: 0.038
    }
}
