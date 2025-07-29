using Sayax.Application.DTOs;
using Sayax.Application.Interfaces;
using Sayax.Application.Repositories;
using Sayax.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var ptfPrices = _priceRepo.GetPtfPricesByMonth(period);
            var staticPrices = _priceRepo.GetStaticPrices();

            var btvReports = new List<BtvReportDto>();

            foreach (var customer in customers)
            {
                foreach (var meter in customer.Meters)
                {
                    var consumptions = _consumptionRepo.GetConsumptionsByMeterAndMonth(meter.ConsumptionType, period);
                    var totalConsumption = consumptions.Sum(c => c.ConsumptionMWh);
                    decimal energyCost = 0;

                    if (meter.SalesMethod.Contains("PTF"))
                    {
                        foreach (var c in consumptions)
                        {
                            var ptf = ptfPrices.FirstOrDefault(p => p.DateTime.Date == c.Date.Date && p.DateTime.Hour == c.Hour);
                            if (ptf != null)
                                energyCost += c.ConsumptionMWh * ptf.PricePerMWh;
                        }
                    }
                    else if (meter.SalesMethod.Contains("Tarife"))
                    {
                        var energyTariffDict = staticPrices.EnergyTariffs.ToDictionary(e => e.AboneGroup, e => e.Price);

                        energyCost = totalConsumption * energyTariffDict[meter.TariffName];

                    }
                    else if (meter.SalesMethod.Contains("YEK"))
                    {
                        energyCost = totalConsumption * staticPrices.YekPrice;
                    }

                    // Komisyon / İndirim
                    if (meter.CommissionType == "Percentage")
                        energyCost *= (1 + meter.CommissionValue);
                    else if (meter.CommissionType == "Fixed")
                        energyCost += meter.CommissionValue;

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
    }
}
