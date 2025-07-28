using Microsoft.AspNetCore.Mvc;
using Sayax.Application.Interfaces;
using Sayax.Application.DTOs;

namespace Sayax.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxController : ControllerBase
{
    private readonly ITaxService _taxService;

    public TaxController(ITaxService taxService)
    {
        _taxService = taxService;
    }

    [HttpGet("btv-report")]
    public ActionResult<List<BtvReportDto>> GetBtvReport([FromQuery] DateTime month)
    {
        var result = _taxService.GetBtvReport(month);
        return Ok(result);
    }
}
