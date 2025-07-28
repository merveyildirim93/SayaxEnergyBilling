using Sayax.Application.Interfaces;
using Sayax.Domain.Entities;
using Sayax.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sayax.Application.Repositories;

namespace Sayax.Infrastructure.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly SayaxDbContext _context;

    public PriceRepository(SayaxDbContext context)
    {
        _context = context;
    }

    public List<PtfPrice> GetPtfPricesByMonth(DateTime period)
    {
        var start = new DateTime(period.Year, period.Month, 1);
        var end = start.AddMonths(1);
        return _context.PtfPrices
            .Where(p => p.DateTime >= start && p.DateTime < end)
            .ToList();
    }

    public StaticPrices GetStaticPrices()
    {
        return _context.StaticPrices
            .Include(sp => sp.EnergyTariffs)
            .Include(sp => sp.DistributionTariffs)
            .First(); // yalnızca 1 satır olduğunu varsayıyoruz
    }


}
