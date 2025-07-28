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
            // Diğer entity konfigürasyonları...

            modelBuilder.Entity<StaticPrices>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<HourlyConsumption>()
                .HasKey(h => new { h.MeterId, h.Date, h.Hour });

            modelBuilder.Entity<PtfPrice>()
                .HasNoKey();


            modelBuilder.Entity<StaticPrices>()
        .HasMany(s => s.EnergyTariffs)
        .WithOne(e => e.StaticPrices)
        .HasForeignKey(e => e.StaticPricesId);

            modelBuilder.Entity<StaticPrices>()
                .HasMany(s => s.DistributionTariffs)
                .WithOne(d => d.StaticPrices)
                .HasForeignKey(d => d.StaticPricesId);
        }
    }
}
