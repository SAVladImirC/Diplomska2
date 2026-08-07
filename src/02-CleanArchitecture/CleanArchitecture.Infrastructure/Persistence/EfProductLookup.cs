using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

public class EfProductLookup(AppDbContext context) : IProductLookup
{
    public async Task<Product?> GetByIdAsync(int id) => await context.Products.FindAsync(id);
}
