using Sayax.Domain.Entities;

namespace Sayax.Application.Repositories
{
    public interface IPriceRepository
    {
        Task<List<PtfPrice>> GetPtfPricesByMonthAsync(DateTime period);
        Task<StaticPrices> GetStaticPricesAsync(DateTime period);
    }
}
