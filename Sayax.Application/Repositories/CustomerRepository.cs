using Sayax.Application.Interfaces;
using Sayax.Application.Repositories;
using Sayax.Domain.Entities;
using Sayax.Infrastructure.Data;

namespace Sayax.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly SayaxDbContext _context;

    public CustomerRepository(SayaxDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetCustomerById(int id)
    {
        return _context.Customers
            .Where(c => c.Id == id)
            .Select(c => new Customer
            {
                Id = c.Id,
                Name = c.Name,
                Meters = _context.Meters
                    .Where(m => m.CustomerId == c.Id)
                    .ToList()
            }).FirstOrDefault();
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return _context.Customers.ToList();
    }
}
