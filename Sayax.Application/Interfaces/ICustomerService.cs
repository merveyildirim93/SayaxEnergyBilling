using Sayax.Application.DTOs;
using Sayax.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayax.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomers();
    }
}
