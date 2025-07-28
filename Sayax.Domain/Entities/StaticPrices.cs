using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Domain.Entities
{
    public class StaticPrices
    {
        public int Id { get; set; }

        public decimal YekPrice { get; set; }

        public ICollection<EnergyTariffs> EnergyTariffs { get; set; }
        public ICollection<DistributionTariffs> DistributionTariffs { get; set; }
    }

}
