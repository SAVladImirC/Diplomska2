using Microsoft.EntityFrameworkCore;
using NTier.Data.Entities;

namespace NTier.Data.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context)
{
    public override async Task<Order?> GetByIdAsync(int id) =>
        await Context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

    public override async Task<List<Order>> GetAllAsync() =>
        await Context.Orders.Include(o => o.Items).ToListAsync();

    public override async Task UpdateAsync(Order entity)
    {
        if (entity.Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException(
                $"Order {entity.Id} is already delivered and can no longer be updated.");
        }

        await base.UpdateAsync(entity);
    }

    public override async Task DeleteAsync(int id)
    {
        var order = await Context.Orders.FindAsync(id);
        if (order is null) return;

        if (order.Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException(
                $"Order {id} is already delivered and can no longer be deleted.");
        }

        await base.DeleteAsync(id);
    }
}
