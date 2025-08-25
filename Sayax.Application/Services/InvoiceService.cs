using Microsoft.Extensions.Logging;
using Sayax.Application.DTOs;
using Sayax.Application.Enums;
using Sayax.Application.Interfaces;
using Sayax.Application.Repositories;
using Sayax.Domain.Entities;

namespace Sayax.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IConsumptionRepository _consumptionRepo;
        private readonly IPriceRepository _priceRepo;
        private readonly ILogger<InvoiceService> _logger;


        public InvoiceService(
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

        public async Task<InvoiceResultDto> CalculateInvoiceAsync(InvoiceRequestDto request)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(request.CustomerId);
            if (customer == null)
            {
                _logger.LogError($"Müşteri bulunamadı. CustomerId: {request.CustomerId}");
                return new InvoiceResultDto();
            }
            var staticPrices = await _priceRepo.GetStaticPricesAsync(period);
            var ptfDict = (await _priceRepo.GetPtfPricesByMonthAsync(request.Month))
                .ToDictionary(p => p.DateTime, p => p.PricePerMWh);

            var energyTariffDict = staticPrices.EnergyTariffs
                .ToDictionary(e => e.AboneGroup.Trim().ToLower(), e => e.Price);

            var distributionTariffDict = staticPrices.DistributionTariffs
                .ToDictionary(d => d.AboneGroup.Trim().ToLower(), d => d.Price);

            decimal totalEnergyCost = 0, totalDistributionCost = 0, totalBtv = 0;

            foreach (var meter in customer.Meters)
            {
                var consumptions = await _consumptionRepo.GetConsumptionsByMeterAndMonthAsync(meter.ConsumptionType, request.Month);

                if (consumptions == null || consumptions.Count == 0)
                {
                    _logger.LogError($"Tüketim verisi bulunamadı. MeterId: {meter.Id}, Month: {request.Month}");
                    continue;
                }

                var meterTotalConsumption = consumptions.Sum(c => c.ConsumptionMWh);

                decimal meterEnergyCost = CalculateEnergyCost(meter, consumptions, staticPrices, ptfDict, energyTariffDict);
                decimal meterDistribution = CalculateDistributionCost(meter, meterTotalConsumption, distributionTariffDict);
                decimal meterBtv = meterEnergyCost * meter.BtvRate;

                totalEnergyCost += meterEnergyCost;
                totalDistributionCost += meterDistribution;
                totalBtv += meterBtv;
            }

            var kdvRate = customer.Meters.First().KdvRate;
            var totalKdv = (totalEnergyCost + totalDistributionCost + totalBtv) * kdvRate;

            return new InvoiceResultDto
            {
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                EnergyCost = Math.Round(totalEnergyCost, 2),
                DistributionCost = Math.Round(totalDistributionCost, 2),
                Btv = Math.Round(totalBtv, 2),
                Kdv = Math.Round(totalKdv, 2),
                TotalInvoice = Math.Round(totalEnergyCost + totalDistributionCost + totalBtv + totalKdv, 2)
            };
        }

        public async Task<List<InvoiceResultDto>> CalculateInvoicesForAllCustomersAsync(DateTime month)
        {
            var customers = await _customerRepo.GetAllCustomersAsync();

            var invoices = new List<InvoiceResultDto>();

            foreach (var customer in customers)
            {
                var request = new InvoiceRequestDto
                {
                    CustomerId = customer.Id,
                    Month = month
                };

                var invoice = await CalculateInvoiceAsync(request);
                invoices.Add(invoice);
            }

            return invoices;
        }

        #region Private Calculation Methods

        private decimal CalculateEnergyCost(
            Meter meter,
            List<HourlyConsumption> consumptions,
            StaticPrices staticPrices,
            Dictionary<DateTime, decimal> ptfDict,
            Dictionary<string, decimal> energyTariffDict)
        {
            decimal meterEnergyCost = 0;
            var meterTotalConsumption = consumptions.Sum(c => c.ConsumptionMWh);

            if (meter.SalesMethod.Contains(SalesMethods.Ptf) && meter.SalesMethod.Contains(SalesMethods.Yek))
            {
                foreach (var c in consumptions)
                {
                    var consumptionDateTime = c.Date.AddHours(c.Hour - 1);
                    if (!ptfDict.TryGetValue(consumptionDateTime, out var pricePerMWh))
                    {
                        _logger.LogError($"PTF fiyatı bulunamadı. DateTime: {consumptionDateTime}, MeterId: {meter.Id}");
                        continue;
                    }
                    meterEnergyCost += c.ConsumptionMWh * (pricePerMWh + staticPrices.YekPrice);
                }
            }
            else if (meter.SalesMethod.Contains(SalesMethods.TarifeIndirim))
            {
                if (!energyTariffDict.TryGetValue(meter.TariffName.Trim().ToLower(), out decimal tariffPrice))
                {
                    _logger.LogError($"Enerji tarifesi bulunamadı. TariffName: {meter.TariffName}, MeterId: {meter.Id}");
                }

                meterEnergyCost = meterTotalConsumption * tariffPrice;
                meterEnergyCost *= (1 + meter.CommissionValue);
            }
            else if (meter.SalesMethod.Contains(SalesMethods.Yek) && !meter.SalesMethod.Contains(SalesMethods.Ptf))
            {
                meterEnergyCost = meterTotalConsumption * staticPrices.YekPrice;
            }

            // Sabit/Oran komisyon
            if (meter.CommissionType == CommissionTypes.Fixed)
                meterEnergyCost += meter.CommissionValue;
            else if (meter.CommissionType == CommissionTypes.Percentage && !meter.SalesMethod.Contains(SalesMethods.TarifeIndirim))
                meterEnergyCost *= (1 + meter.CommissionValue);

            return meterEnergyCost;
        }

        private decimal CalculateDistributionCost(Meter meter, decimal meterTotalConsumption, Dictionary<string, decimal> distributionTariffDict)
        {
            var key = meter.TariffName.Trim().ToLower();
            if (!distributionTariffDict.TryGetValue(key, out decimal distPrice))
            {
                _logger.LogError($"Dağıtım tarifesi bulunamadı. TariffName: {meter.TariffName}, MeterId: {meter.Id}");
            }

            return meterTotalConsumption * distPrice;
        }

        #endregion
    }

}
