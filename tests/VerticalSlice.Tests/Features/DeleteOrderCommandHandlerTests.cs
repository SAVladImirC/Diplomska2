using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Entities;
using VerticalSlice.Infrastructure.Persistence;
using VerticalSlice.Infrastructure.Services;
using VerticalSlice.Web.Features.Orders.DeleteOrder;
using Xunit;

namespace VerticalSlice.Tests.Features;

public class DeleteOrderCommandHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private class NoOpEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body) => Task.CompletedTask;
    }

    [Fact]
    public async Task Handle_OnDeliveredOrder_SoftDeletesWithoutThrowing()
    {
        // Same "delivered order" scenario as NTier's LSP-violation test, but there is
        // no shared generic repository here for a status check to break against.
        await using var context = CreateContext();
        var customer = new Customer { Name = "Тест", Email = "test@example.com" };
        var order = new Order { CustomerId = 0, Status = OrderStatus.Delivered };
        context.Customers.Add(customer);
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        order.CustomerId = customer.Id;
        await context.SaveChangesAsync();

        var handler = new DeleteOrderCommandHandler(context, new NoOpEmailService());

        var exception = await Record.ExceptionAsync(() =>
            handler.Handle(new DeleteOrderCommand(order.Id), CancellationToken.None));

        Assert.Null(exception);
        var reloaded = await context.Orders.FirstAsync(o => o.Id == order.Id);
        Assert.True(reloaded.IsDeleted);
    }
}
