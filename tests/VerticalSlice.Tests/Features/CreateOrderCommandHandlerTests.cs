using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Entities;
using VerticalSlice.Infrastructure.Persistence;
using VerticalSlice.Infrastructure.Services;
using VerticalSlice.Web.Features.Orders.CreateOrder;
using Xunit;

namespace VerticalSlice.Tests.Features;

public class CreateOrderCommandHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private class RecordingEmailService : IEmailService
    {
        public int CallCount { get; private set; }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            CallCount++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Handle_OverDiscountThreshold_Applies10PercentDiscountAndSendsConfirmation()
    {
        await using var context = CreateContext();
        var customer = new Customer { Name = "Тест", Email = "test@example.com" };
        var product = new Product { Name = "Widget", UnitPrice = 60m };
        context.Customers.Add(customer);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var emailService = new RecordingEmailService();
        var handler = new CreateOrderCommandHandler(context, emailService);

        var response = await handler.Handle(
            new CreateOrderCommand(customer.Id, [new CreateOrderItem(product.Id, 2)]),
            CancellationToken.None);

        Assert.Equal(120m, response.Subtotal);
        Assert.Equal(12m, response.DiscountApplied);
        Assert.Equal(108m, response.Total);
        Assert.Equal(1, emailService.CallCount);
    }
}
