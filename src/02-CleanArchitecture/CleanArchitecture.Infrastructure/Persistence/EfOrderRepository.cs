using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

/// <summary>
/// One class can implement every tiny role interface -- ISP is about what a
/// *consumer* is forced to depend on, not about how many interfaces a single
/// implementation may satisfy.
/// </summary>
public class EfOrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(int id) =>
        await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

    public async Task<List<Order>> GetAllAsync() =>
        await context.Orders
            .Include(o => o.Items)
            .Where(o => !o.IsDeleted)
            .ToListAsync();

    public async Task AddAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int orderId)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return;

        order.MarkDeleted();
        await context.SaveChangesAsync();
    }
}
