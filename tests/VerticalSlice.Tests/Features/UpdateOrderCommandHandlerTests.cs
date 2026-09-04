using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Entities;
using VerticalSlice.Infrastructure.Persistence;
using VerticalSlice.Web.Features.Orders.UpdateOrder;
using Xunit;

namespace VerticalSlice.Tests.Features;

public class UpdateOrderCommandHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_OnDeliveredOrder_Throws()
    {
        await using var context = CreateContext();
        var customer = new Customer { Name = "Тест", Email = "test@example.com" };
        var product = new Product { Name = "Widget", UnitPrice = 60m };
        var order = new Order { CustomerId = 0, Status = OrderStatus.Delivered };
        context.Customers.Add(customer);
        context.Products.Add(product);
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var handler = new UpdateOrderCommandHandler(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new UpdateOrderCommand(order.Id, customer.Id, [new UpdateOrderItem(product.Id, 1)]),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_MatchesCreateOrder_ForTheSameDiscountRule()
    {
        await using var context = CreateContext();
        var customer = new Customer { Name = "Тест", Email = "test@example.com" };
        var product = new Product { Name = "Widget", UnitPrice = 60m };
        var order = new Order { CustomerId = 0, Status = OrderStatus.Pending };
        context.Customers.Add(customer);
        context.Products.Add(product);
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var handler = new UpdateOrderCommandHandler(context);

        var response = await handler.Handle(
            new UpdateOrderCommand(order.Id, customer.Id, [new UpdateOrderItem(product.Id, 2)]),
            CancellationToken.None);

        Assert.Equal(120m, response.Subtotal);
        Assert.Equal(12m, response.DiscountApplied);
        Assert.Equal(108m, response.Total);
    }
}
