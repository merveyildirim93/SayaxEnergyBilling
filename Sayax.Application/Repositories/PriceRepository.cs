using Microsoft.EntityFrameworkCore;
using Sayax.Application.Repositories;
using Sayax.Domain.Entities;
using Sayax.Infrastructure.Data;
using StackExchange.Redis;
using System.Text.Json;

namespace Sayax.Infrastructure.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly SayaxDbContext _context;
    private readonly RedisContext _rediscontext;
    public PriceRepository(SayaxDbContext context, RedisContext rediscontext)
    {
        _context = context;
        _rediscontext = rediscontext;
    }

    public async Task<List<PtfPrice>> GetPtfPricesByMonthAsync(DateTime period)
    {
        var start = new DateTime(period.Year, period.Month, 1);
        var end = start.AddMonths(1);
        return await _context.PtfPrices
            .Where(p => p.DateTime >= start && p.DateTime < end)
            .ToListAsync();
    }

    public async Task<StaticPrices> GetStaticPricesAsync(DateTime period)
    {
        string cacheKey = $"StaticPrices:{period:yyyy-MM}";
        var staticPrices = new StaticPrices();

        // Redis'ten bilgiyi çektik
        var cachedValue = await _rediscontext.Db.StringGetAsync(cacheKey);
        if (cachedValue.HasValue)
        {
            staticPrices = JsonSerializer.Deserialize<StaticPrices>(cachedValue)!;
        }
        else
        {
            // Redis'te yoksa DB'den çektik
            staticPrices = await _context.StaticPrices
                .Include(sp => sp.EnergyTariffs)
                .Include(sp => sp.DistributionTariffs)
                .FirstAsync();


            // JSON olarak Redis'e yazdırıyoruz (TTL 1 saat ayarlandı)
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                WriteIndented = false
            };

            string json = JsonSerializer.Serialize(staticPrices, options);
            await _rediscontext.Db.StringSetAsync(cacheKey, json, TimeSpan.FromHours(1));
        }

        return staticPrices;
    }
}

