using Microsoft.AspNetCore.Mvc;
using Sayax.Application.DTOs;
using Sayax.Application.Interfaces;

namespace Sayax.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<InvoiceResultDto>> CalculateAsync([FromBody] InvoiceRequestDto request)
    {
        var result = await _invoiceService.CalculateInvoiceAsync(request);
        return Ok(result);
    }

    [HttpPost("calculate-all")]
    public async Task<IActionResult> CalculateInvoicesForAll([FromQuery] DateTime month)
    {
        var result = await _invoiceService.CalculateInvoicesForAllCustomersAsync(month);
        return Ok(result);
    }
}
