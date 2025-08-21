using Microsoft.EntityFrameworkCore;
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

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.Meters)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _context.Customers
            .Include(c => c.Meters)
            .ToListAsync();
    }

}
