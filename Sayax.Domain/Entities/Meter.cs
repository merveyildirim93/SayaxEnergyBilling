using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Domain.Entities
{
    public class Meter
    {
        public string Id { get; set; } // Sayaç ID'si: S1, S2, S3 gibi
        public string SalesMethod { get; set; } // PTF+YEK, Tarife, vs.
        public string AboneGroup { get; set; } // Sanayi, Ticarethane
        public decimal BtvRate { get; set; }   // Belediye tüketim vergisi oranı
        public decimal KdvRate { get; set; }   // KDV oranı
        public string Municipality { get; set; } // Örnek: Kadıköy
        public string TariffName { get; set; }   // Sanayi vs.

        public decimal CommissionValue { get; set; }  // Sabit veya yüzde olarak
        public string CommissionType { get; set; }    // "Percentage" veya "Fixed"

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
