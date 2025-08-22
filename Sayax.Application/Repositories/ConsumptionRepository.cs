using Microsoft.EntityFrameworkCore;
using Sayax.Application.Repositories;
using Sayax.Domain.Entities;
using Sayax.Infrastructure.Data;

namespace Sayax.Infrastructure.Repositories;

public class ConsumptionRepository : IConsumptionRepository
{
    private readonly SayaxDbContext _context;

    public ConsumptionRepository(SayaxDbContext context)
    {
        _context = context;
    }

    public async Task<List<HourlyConsumption>> GetConsumptionsByMeterAndMonthAsync(string meterId, DateTime period)
    {
        var startDate = new DateTime(period.Year, period.Month, period.Day);
        var endDate = startDate.AddMonths(1);

        return await _context.HourlyConsumptions
            .Where(c => c.MeterId == meterId && c.Date >= startDate && c.Date < endDate)
            .ToListAsync();
    }
}

