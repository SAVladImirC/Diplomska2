using Microsoft.EntityFrameworkCore;
using NTier.Data;
using NTier.Data.Entities;
using NTier.Data.Repositories;
using Xunit;

namespace NTier.Tests;

public class OrderRepositoryLspTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task UpdateAsync_OnDeliveredOrder_ThrowsAndBreaksTheGenericContract()
    {
        await using var context = CreateContext();
        var order = new Order { CustomerId = 1, Status = OrderStatus.Delivered };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        IRepository<Order> repository = new OrderRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateAsync(order));
    }

    [Fact]
    public async Task DeleteAsync_OnDeliveredOrder_ThrowsAndBreaksTheGenericContract()
    {
        await using var context = CreateContext();
        var order = new Order { CustomerId = 1, Status = OrderStatus.Delivered };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        IRepository<Order> repository = new OrderRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.DeleteAsync(order.Id));
    }

    [Fact]
    public async Task UpdateAsync_OnPendingOrder_SucceedsNormally()
    {
        await using var context = CreateContext();
        var order = new Order { CustomerId = 1, Status = OrderStatus.Pending };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        IRepository<Order> repository = new OrderRepository(context);
        order.Status = OrderStatus.Confirmed;

        await repository.UpdateAsync(order);

        var reloaded = await repository.GetByIdAsync(order.Id);
        Assert.Equal(OrderStatus.Confirmed, reloaded!.Status);
    }
}
