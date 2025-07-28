using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Domain.Entities
{
    public class DistributionTariffs
    {
        public int Id { get; set; }
        public string AboneGroup { get; set; }
        public decimal Price { get; set; }

        public int StaticPricesId { get; set; }
        public StaticPrices StaticPrices { get; set; }
    }

}
