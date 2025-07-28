using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Bir müşterinin birden fazla sayacı olabilir
        public List<Meter> Meters { get; set; } = new();
    }
}
