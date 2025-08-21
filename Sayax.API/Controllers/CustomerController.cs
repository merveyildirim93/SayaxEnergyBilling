using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Sayax.Application.Interfaces;
using Sayax.Application.Services;
using Sayax.Domain.Entities;

namespace Sayax.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
                _customerService = customerService;
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _customerService.GetAllCustomers();
            return Ok(result);
        }
    }
}
