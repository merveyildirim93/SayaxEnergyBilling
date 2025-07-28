using Sayax.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Application.Repositories
{
    public interface IPriceRepository
    {
        List<PtfPrice> GetPtfPricesByMonth(DateTime period);
        StaticPrices GetStaticPrices(); // Sabit YEK, Tarife, Dağıtım bilgileri
    }
}
