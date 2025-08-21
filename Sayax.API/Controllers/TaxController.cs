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

    [HttpPost("btv-report")]
    public async Task<ActionResult<List<BtvReportDto>>> GetBtvReportAsync([FromQuery] DateTime month)
    {
        var result = await _taxService.GetBtvReportAsync(month);
        return Ok(result);
    }
}
