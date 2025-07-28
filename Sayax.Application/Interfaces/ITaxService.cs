using Sayax.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Application.Interfaces
{
    public interface ITaxService
    {
        List<BtvReportDto> GetBtvReport(DateTime period);
    }
}
