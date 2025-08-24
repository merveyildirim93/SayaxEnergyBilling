using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<InvoiceService> _logger;

        public TaxService(
            ICustomerRepository customerRepo,
            IConsumptionRepository consumptionRepo,
            IPriceRepository priceRepo,
            ILogger<InvoiceService> logger)
        {
            _customerRepo = customerRepo;
            _consumptionRepo = consumptionRepo;
            _priceRepo = priceRepo;
            _logger = logger;
        }

        public async Task<List<BtvReportDto>> GetBtvReportAsync(DateTime period)
        {
            try
            {
                var customers = await _customerRepo.GetAllCustomersAsync();
                if (customers == null || !customers.Any())
                {
                    _logger.LogError($"BTV raporu hesaplanamadı: müşteri bulunamadı. Period: {period}");
                    return new List<BtvReportDto>();
                }

                var meterIds = customers.SelectMany(c => c.Meters.Select(m => m.ConsumptionType)).Distinct().ToList();

                var allConsumptions = await _consumptionRepo.GetConsumptionsByMeterAndMonthForBtvAsync(meterIds, period);

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
                        if (!allConsumptions.TryGetValue(meter.ConsumptionType, out var meterConsumptions) || !meterConsumptions.Any())
                        {
                            _logger.LogError($"Tüketim verisi bulunamadı. CustomerId: {customer.Id}, MeterId: {meter.Id}, Period: {period}");
                            continue;
                        }

                        var totalConsumption = meterConsumptions.Sum(c => c.ConsumptionMWh);

                        decimal energyCost;
                        try
                        {
                            energyCost = CalculateEnergyCostForBtv(meter, meterConsumptions, totalConsumption, ptfPrices, staticPrices, energyTariffDict);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Enerji maliyeti hesaplanamadı. CustomerId: {customer.Id}, MeterId: {meter.Id}, Period: {period}");
                            continue;
                        }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"BTV raporu genel hata. Period: {period}");
                throw;
            }
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
                    if (!ptfPrices.TryGetValue(key, out var ptfPrice))
                    {
                        _logger.LogError($"PTF fiyatı bulunamadı. Tarih: {c.Date}, Saat: {c.Hour}, MeterId: {meter.Id}");
                        continue;
                        //buraya iş kuralları getirilebilir.
                    }

                    energyCost += c.ConsumptionMWh * ptfPrice;
                }
            }
            else if (meter.SalesMethod.Contains("Tarife"))
            {
                if (!energyTariffDict.TryGetValue(meter.TariffName.Trim().ToLower(), out var tariffPrice))
                {
                    _logger.LogError($"Dağıtım tarifesi bulunamadı. Tarife: {meter.TariffName}, MeterId: {meter.Id}");
                    //buraya iş kuralları getirilebilir.
                }

                energyCost = totalConsumption * tariffPrice;
            }
            else if (meter.SalesMethod.Contains("YEK"))
            {
                if (staticPrices.YekPrice <= 0)
                {
                    _logger.LogError($"YEK fiyatı geçersiz. YekPrice: {staticPrices.YekPrice}");
                }

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
