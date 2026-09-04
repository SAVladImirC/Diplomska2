using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CleanArchitecture.Tests.Infrastructure;

public class EfOrderRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task SoftDeleteAsync_OnDeliveredOrder_SucceedsWhereNTierWouldThrow()
    {
        await using var context = CreateContext();
        var repository = new EfOrderRepository(context);

        var order = Order.Create(1, [new OrderItem(1, 1, new Money(50m))]);
        order.MarkDelivered();
        await repository.AddAsync(order);
        await context.SaveChangesAsync();

        await repository.SoftDeleteAsync(order.Id);
        await context.SaveChangesAsync();

        var reloaded = await context.Orders.FirstAsync(o => o.Id == order.Id);
        Assert.True(reloaded.IsDeleted);
    }

    [Fact]
    public async Task GetAllAsync_ExcludesSoftDeletedOrders()
    {
        await using var context = CreateContext();
        var repository = new EfOrderRepository(context);

        var order = Order.Create(1, [new OrderItem(1, 1, new Money(50m))]);
        await repository.AddAsync(order);
        await context.SaveChangesAsync();
        await repository.SoftDeleteAsync(order.Id);
        await context.SaveChangesAsync();

        var all = await repository.GetAllAsync();

        Assert.Empty(all);
    }
}
