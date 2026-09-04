using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.UseCases;

public class SendOrderConfirmationUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_SendsToTheOrdersCustomer()
    {
        var order = Order.Create(7, [new OrderItem(1, 1, new Money(10m))]);
        var orders = new Mock<IGetOrders>();
        var customers = new Mock<ICustomerLookup>();
        var email = new Mock<IEmailService>();
        orders.Setup(o => o.GetByIdAsync(order.Id)).ReturnsAsync(order);
        customers.Setup(c => c.GetByIdAsync(7)).ReturnsAsync(new Customer { Id = 7, Email = "ana@example.com" });

        await new SendOrderConfirmationUseCase(orders.Object, customers.Object, email.Object).ExecuteAsync(order.Id);

        email.Verify(e => e.SendEmailAsync("ana@example.com", "Order Confirmation", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UnknownOrder_Throws()
    {
        var useCase = new SendOrderConfirmationUseCase(
            Mock.Of<IGetOrders>(), Mock.Of<ICustomerLookup>(), Mock.Of<IEmailService>());

        await Assert.ThrowsAsync<OrderDomainException>(() => useCase.ExecuteAsync(42));
    }
}
