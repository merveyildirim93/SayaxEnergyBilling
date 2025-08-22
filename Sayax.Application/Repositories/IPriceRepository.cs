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
        Task<List<PtfPrice>> GetPtfPricesByMonthAsync(DateTime period);
        Task<StaticPrices> GetStaticPricesAsync(); // Sabit YEK, Tarife, Dağıtım bilgileri
    }
}
