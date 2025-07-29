using Sayax.Application.DTOs;
using Sayax.Application.Interfaces;
using Sayax.Application.Repositories;
using Sayax.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IConsumptionRepository _consumptionRepo;
        private readonly IPriceRepository _priceRepo;

        public InvoiceService(
            ICustomerRepository customerRepo,
            IConsumptionRepository consumptionRepo,
            IPriceRepository priceRepo)
        {
            _customerRepo = customerRepo;
            _consumptionRepo = consumptionRepo;
            _priceRepo = priceRepo;
        }

        public async Task<InvoiceResultDto> CalculateInvoiceAsync(InvoiceRequestDto request)
        {
            var customer = await _customerRepo.GetCustomerById(request.CustomerId);
            if (customer == null)
                throw new Exception("Müşteri bulunamadı");

            var staticPrices = _priceRepo.GetStaticPrices();
            var ptfPrices = _priceRepo.GetPtfPricesByMonth(request.Month);

            decimal totalEnergyCost = 0;
            decimal totalDistributionCost = 0;
            decimal totalBtv = 0;

            foreach (var meter in customer.Meters)
            {
                var consumptions = _consumptionRepo.GetConsumptionsByMeterAndMonth(meter.ConsumptionType, request.Month);
                var meterTotalConsumption = consumptions.Sum(c => c.ConsumptionMWh);
                decimal meterEnergyCost = 0;

                // 1. Enerji bedeli hesabı
                if (meter.SalesMethod.Contains("PTF") && meter.SalesMethod.Contains("YEK"))
                {
                    foreach (var c in consumptions)
                    {
                        var ptf = ptfPrices.FirstOrDefault(p => p.DateTime.Date == c.Date.Date && p.DateTime.Hour == c.Hour);
                        if (ptf != null)
                        {
                            meterEnergyCost += c.ConsumptionMWh * (ptf.PricePerMWh + staticPrices.YekPrice);
                        }
                    }
                }
                else if (meter.SalesMethod.Contains("Tarife - %İndirim"))
                {
                    var energyTariffDict = staticPrices.EnergyTariffs
                        .ToDictionary(e => e.AboneGroup.Trim().ToLower(), e => e.Price);

                    if (energyTariffDict.TryGetValue(meter.TariffName.Trim().ToLower(), out decimal tariffPrice))
                    {
                        meterEnergyCost = meterTotalConsumption * tariffPrice;
                        meterEnergyCost *= (1 + meter.CommissionValue); // indirim değeri -0.03 gibi negatif
                    }
                    else
                    {
                        throw new Exception($"Tarife bulunamadı: {meter.TariffName}");
                    }
                }
                else if (meter.SalesMethod.Contains("YEK") && !meter.SalesMethod.Contains("PTF"))
                {
                    meterEnergyCost = meterTotalConsumption * staticPrices.YekPrice;
                }

                // 2. Sabit komisyon (Yalnızca PTF + YEK + Komisyon gibi durumlarda Fixed olarak gelir)
                if (meter.CommissionType == "Fixed")
                    meterEnergyCost += meter.CommissionValue;
                else if (meter.CommissionType == "Percentage" && !meter.SalesMethod.Contains("Tarife - %İndirim"))
                    meterEnergyCost *= (1 + meter.CommissionValue); // %5 → 1.05 çarpılır

                // 3. Dağıtım bedeli
                decimal meterDistribution = 0;
                var distributionTariffDict = staticPrices.DistributionTariffs
                    .ToDictionary(d => d.AboneGroup.Trim().ToLower(), d => d.Price);

                var tariffKey = meter.TariffName.Trim().ToLower();
                if (distributionTariffDict.TryGetValue(tariffKey, out decimal distTariff))
                {
                    meterDistribution = meterTotalConsumption * distTariff;
                }
                else
                {
                    throw new Exception($"Dağıtım tarifesi bulunamadı: {meter.TariffName}");
                }

                // 4. BTV hesaplama
                var meterBtv = meterEnergyCost * meter.BtvRate;

                totalEnergyCost += meterEnergyCost;
                totalDistributionCost += meterDistribution;
                totalBtv += meterBtv;
            }

            // 5. KDV ve toplam fatura
            var totalKdvBase = totalEnergyCost + totalDistributionCost + totalBtv;
            var totalKdv = totalKdvBase * customer.Meters.Max(m => m.KdvRate); // Aynı müşteri için sabit olmalı
            var totalInvoice = totalKdvBase + totalKdv;

            return new InvoiceResultDto
            {
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                EnergyCost = Math.Round(totalEnergyCost, 2),
                DistributionCost = Math.Round(totalDistributionCost, 2),
                Btv = Math.Round(totalBtv, 2),
                Kdv = Math.Round(totalKdv, 2),
                TotalInvoice = Math.Round(totalInvoice, 2)
            };
        }


        public async Task<InvoiceResultDto> CalculateInvoiceAsync_Old(InvoiceRequestDto request)
        {
            var customer = await _customerRepo.GetCustomerById(request.CustomerId);
            if (customer == null)
                throw new Exception("Müşteri bulunamadı");

            var staticPrices = _priceRepo.GetStaticPrices();
            var ptfPrices = _priceRepo.GetPtfPricesByMonth(request.Month);

            decimal totalEnergyCost = 0;
            decimal totalDistributionCost = 0;
            decimal totalBtv = 0;
            decimal totalKdvBase = 0;

            foreach (var meter in customer.Meters)
            {
                var consumptions = _consumptionRepo.GetConsumptionsByMeterAndMonth(meter.ConsumptionType, request.Month);
                var meterTotalConsumption = consumptions.Sum(c => c.ConsumptionMWh);

                decimal meterEnergyCost = 0;

                if (meter.SalesMethod.Contains("PTF"))
                {
                    foreach (var c in consumptions)
                    {
                        var ptf = ptfPrices.FirstOrDefault(p => p.DateTime.Date == c.Date.Date && p.DateTime.Hour == c.Hour);
                        if (ptf != null)
                            meterEnergyCost += c.ConsumptionMWh * ptf.PricePerMWh;
                    }
                }
                else if (meter.SalesMethod.Contains("Tarife"))
                {
                    var energyTariffDict = staticPrices.EnergyTariffs
                        .ToDictionary(e => e.AboneGroup, e => e.Price);

                    if (energyTariffDict.TryGetValue(meter.TariffName, out decimal tariffPrice))
                    {
                        meterEnergyCost = meterTotalConsumption * tariffPrice;
                    }
                    else
                    {
                        throw new Exception($"Tarife bulunamadı: {meter.TariffName}");
                    }
                }


                else if (meter.SalesMethod.Contains("YEK"))
                {
                    meterEnergyCost = meterTotalConsumption * staticPrices.YekPrice;
                }

                // Komisyon / İndirim
                if (meter.CommissionType == "Percentage")
                    meterEnergyCost *= (1 + meter.CommissionValue); // örn: %5 komisyon → +%5
                else if (meter.CommissionType == "Fixed")
                    meterEnergyCost += meter.CommissionValue;

                decimal meterDistribution = 0;
                var distributionTariffDict = staticPrices.DistributionTariffs
    .ToDictionary(d => d.AboneGroup.Trim().ToLower(), d => d.Price);

                var tariffKey = meter.TariffName.Trim().ToLower();

                if (distributionTariffDict.TryGetValue(tariffKey, out decimal distTariff))
                {
                    meterDistribution = meterTotalConsumption * distTariff;
                }
                else
                {
                    throw new Exception($"Dağıtım tarifesi bulunamadı: {meter.TariffName}");
                }

                var meterBtv = meterEnergyCost * meter.BtvRate;

                totalEnergyCost += meterEnergyCost;
                totalDistributionCost += meterDistribution;
                totalBtv += meterBtv;
            }

            totalKdvBase = totalEnergyCost + totalDistributionCost + totalBtv;
            var totalKdv = totalKdvBase * customer.Meters.Max(m => m.KdvRate); // aynı müşteri için sabit olmalı
            var totalInvoice = totalKdvBase + totalKdv;

            return new InvoiceResultDto
            {
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                EnergyCost = Math.Round(totalEnergyCost, 2),
                DistributionCost = Math.Round(totalDistributionCost, 2),
                Btv = Math.Round(totalBtv, 2),
                Kdv = Math.Round(totalKdv, 2),
                TotalInvoice = Math.Round(totalInvoice, 2)
            };
        }

        public async Task<List<InvoiceResultDto>> CalculateInvoicesForAllCustomersAsync(DateTime month)
        {
            var customers = await _customerRepo.GetAllCustomersAsync();
            var resultList = new List<InvoiceResultDto>();

            foreach (var customer in customers)
            {
                var result = await 
                    CalculateInvoiceAsync(new InvoiceRequestDto
                {
                    CustomerId = customer.Id,
                    Month = month
                });

                resultList.Add(result);
            }

            return resultList;
        }

    }
}
