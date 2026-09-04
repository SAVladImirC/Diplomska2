using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

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

    public Task AddAsync(Order order)
    {
        context.Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public async Task SoftDeleteAsync(int orderId)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        order?.MarkDeleted();
    }
}
