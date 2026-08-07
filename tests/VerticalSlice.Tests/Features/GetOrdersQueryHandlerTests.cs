using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Entities;
using VerticalSlice.Infrastructure.Persistence;
using VerticalSlice.Web.Features.Orders.GetOrders;
using Xunit;

namespace VerticalSlice.Tests.Features;

public class GetOrdersQueryHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ExcludesSoftDeletedOrders()
    {
        await using var context = CreateContext();
        context.Orders.AddRange(
            new Order { CustomerId = 1, IsDeleted = false },
            new Order { CustomerId = 1, IsDeleted = true });
        await context.SaveChangesAsync();

        var handler = new GetOrdersQueryHandler(context);
        var result = await handler.Handle(new GetOrdersQuery(), CancellationToken.None);

        Assert.Single(result);
    }
}
