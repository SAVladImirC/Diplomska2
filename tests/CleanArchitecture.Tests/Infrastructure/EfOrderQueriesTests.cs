using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.ValueObjects;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CleanArchitecture.Tests.Infrastructure;

public class EfOrderQueriesTests
{
    [Fact]
    public async Task GetAllAsync_ProjectsSubtotalAndSkipsSoftDeleted()
    {
        await using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        var kept = Order.Create(1, [new OrderItem(1, 2, new Money(60m)), new OrderItem(2, 1, new Money(5m))]);
        var deleted = Order.Create(1, [new OrderItem(1, 1, new Money(10m))]);
        deleted.MarkDeleted();
        context.Orders.AddRange(kept, deleted);
        await context.SaveChangesAsync();

        var result = await new EfOrderQueries(context).GetAllAsync();

        var row = Assert.Single(result);
        Assert.Equal(kept.Id, row.Id);
        Assert.Equal(125m, row.Subtotal);
        Assert.Equal(OrderStatus.Pending, row.Status);
    }
}
