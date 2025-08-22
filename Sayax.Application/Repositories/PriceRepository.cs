using Microsoft.EntityFrameworkCore;
using Sayax.Application.Repositories;
using Sayax.Domain.Entities;
using Sayax.Infrastructure.Data;

namespace Sayax.Infrastructure.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly SayaxDbContext _context;
    public PriceRepository(SayaxDbContext context) => _context = context;

    public async Task<List<PtfPrice>> GetPtfPricesByMonthAsync(DateTime period)
    {
        var start = new DateTime(period.Year, period.Month, 1);
        var end = start.AddMonths(1);
        return await _context.PtfPrices
            .Where(p => p.DateTime >= start && p.DateTime < end)
            .ToListAsync();
    }

    public async Task<StaticPrices> GetStaticPricesAsync()
    {
        return await _context.StaticPrices
            .Include(sp => sp.EnergyTariffs)
            .Include(sp => sp.DistributionTariffs)
            .FirstAsync();
    }
}

