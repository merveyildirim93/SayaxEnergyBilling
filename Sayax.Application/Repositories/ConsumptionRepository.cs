using Sayax.Application.Interfaces;
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

    public List<HourlyConsumption> GetConsumptionsByMeterAndMonth(string meterId, DateTime period)
    {
        var startDate = new DateTime(period.Year, period.Month, 1);
        var endDate = startDate.AddMonths(1);

        return _context.HourlyConsumptions
            .Where(c => c.MeterId == meterId && c.Date >= startDate && c.Date < endDate)
            .ToList();
    }
}
