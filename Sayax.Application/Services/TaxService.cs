using Sayax.Application.DTOs;
using Sayax.Application.Enums;
using Sayax.Application.Interfaces;
using Sayax.Application.Repositories;
using Sayax.Domain.Entities;

namespace Sayax.Application.Services
{
    public class TaxService : ITaxService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IConsumptionRepository _consumptionRepo;
        private readonly IPriceRepository _priceRepo;

        public TaxService(
            ICustomerRepository customerRepo,
            IConsumptionRepository consumptionRepo,
            IPriceRepository priceRepo)
        {
            _customerRepo = customerRepo;
            _consumptionRepo = consumptionRepo;
            _priceRepo = priceRepo;
        }

        public async Task<List<BtvReportDto>> GetBtvReportAsync(DateTime period)
        {
            var customers = await _customerRepo.GetAllCustomersAsync();
            if (customers == null || !customers.Any())
                throw new Exception("Müşteri bulunamadı");

            var ptfPrices = (await _priceRepo.GetPtfPricesByMonthAsync(period))
                .ToDictionary(p => p.DateTime, p => p.PricePerMWh);

            var staticPrices = await _priceRepo.GetStaticPricesAsync();

            var energyTariffDict = staticPrices.EnergyTariffs
                .ToDictionary(e => e.AboneGroup.Trim().ToLower(), e => e.Price);

            var btvReports = new List<BtvReportDto>();

            foreach (var customer in customers)
            {
                foreach (var meter in customer.Meters)
                {
                    var consumptions = await _consumptionRepo.GetConsumptionsByMeterAndMonthAsync(meter.ConsumptionType, period);
                    var totalConsumption = consumptions.Sum(c => c.ConsumptionMWh);

                    decimal energyCost = CalculateEnergyCostForBtv(meter, consumptions, totalConsumption, ptfPrices, staticPrices, energyTariffDict);
                    decimal btv = energyCost * meter.BtvRate;

                    btvReports.Add(new BtvReportDto
                    {
                        CustomerId = customer.Id,
                        CustomerName = customer.Name,
                        Municipality = meter.Municipality,
                        BtvAmount = Math.Round(btv, 2)
                    });
                }
            }

            return btvReports;
        }

        #region Private Helpers

        private decimal CalculateEnergyCostForBtv(
            Meter meter,
            List<HourlyConsumption> consumptions,
            decimal totalConsumption,
            Dictionary<DateTime, decimal> ptfPrices,
            StaticPrices staticPrices,
            Dictionary<string, decimal> energyTariffDict)
        {
            decimal energyCost = 0;

            if (meter.SalesMethod.Contains("PTF"))
            {
                foreach (var c in consumptions)
                {
                    var key = c.Date.AddHours(c.Hour - 1);
                    if (ptfPrices.TryGetValue(key, out var ptfPrice))
                        energyCost += c.ConsumptionMWh * ptfPrice;
                }
            }
            else if (meter.SalesMethod.Contains("Tarife"))
            {
                if (!energyTariffDict.TryGetValue(meter.TariffName.Trim().ToLower(), out var tariffPrice))
                    throw new Exception("Dağıtım tarifesi bulunamadı: " + meter.TariffName);

                energyCost = totalConsumption * tariffPrice;
            }
            else if (meter.SalesMethod.Contains("YEK"))
            {
                energyCost = totalConsumption * staticPrices.YekPrice;
            }

            // Komisyon / İndirim
            if (meter.CommissionType == CommissionTypes.Percentage)
                energyCost *= (1 + meter.CommissionValue);
            else if (meter.CommissionType == CommissionTypes.Fixed)
                energyCost += meter.CommissionValue;

            return energyCost;
        }

        #endregion
    }
}
