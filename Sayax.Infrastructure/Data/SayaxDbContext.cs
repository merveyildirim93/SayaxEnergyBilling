using Microsoft.EntityFrameworkCore;
using Sayax.Domain.Entities;
using System.Text.Json;

namespace Sayax.Infrastructure.Data
{
    public class SayaxDbContext : DbContext
    {
        public SayaxDbContext(DbContextOptions<SayaxDbContext> options)
            : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<HourlyConsumption> HourlyConsumptions { get; set; }
        public DbSet<PtfPrice> PtfPrices { get; set; }
        public DbSet<StaticPrices> StaticPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StaticPrices>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<HourlyConsumption>()
                .HasKey(h => new { h.MeterId, h.Date, h.Hour });

            modelBuilder.Entity<HourlyConsumption>()
                .Property(h => h.ConsumptionMWh)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PtfPrice>()
                .HasNoKey();

            //modelBuilder.Entity<StaticPrices>()
            //    .HasMany(s => s.EnergyTariffs)
            //    .WithOne(e => e.StaticPrices)
            //    .HasForeignKey(e => e.StaticPricesId);

            modelBuilder.Entity<EnergyTariffs>()
                .Property(e => e.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DistributionTariffs>()
                .Property(d => d.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Meter>()
                .Property(m => m.BtvRate)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Meter>()
               .Property(m => m.KdvRate)
               .HasPrecision(18, 4);

            modelBuilder.Entity<Meter>()
                .Property(m => m.CommissionValue)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PtfPrice>()
                .Property(m => m.PricePerMWh)
                .HasPrecision(18, 4);

            modelBuilder.Entity<StaticPrices>()
               .Property(m => m.YekPrice)
               .HasPrecision(18, 4);
        }

    }
}
