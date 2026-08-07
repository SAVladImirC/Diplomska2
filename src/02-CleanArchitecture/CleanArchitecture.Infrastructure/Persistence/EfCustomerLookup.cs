using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

public class EfCustomerLookup(AppDbContext context) : ICustomerLookup
{
    public async Task<Customer?> GetByIdAsync(int id) => await context.Customers.FindAsync(id);
}
