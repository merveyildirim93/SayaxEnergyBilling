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
    public ActionResult<InvoiceResultDto> Calculate([FromBody] InvoiceRequestDto request)
    {
        var result = _invoiceService.CalculateInvoice(request.CustomerId, request.Month);
        return Ok(result);
    }
}
