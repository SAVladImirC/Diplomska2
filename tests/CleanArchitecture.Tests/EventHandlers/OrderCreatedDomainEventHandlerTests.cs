using CleanArchitecture.Application.EventHandlers;
using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using CleanArchitecture.Domain.ValueObjects;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.EventHandlers;

public class OrderCreatedDomainEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_TriggersConfirmationForTheCreatedOrder()
    {
        var order = Order.Create(1, [new OrderItem(1, 1, new Money(10m))]);
        var sendConfirmation = new Mock<ISendOrderConfirmationUseCase>();

        await new OrderCreatedDomainEventHandler(sendConfirmation.Object)
            .HandleAsync(new OrderCreatedDomainEvent(order));

        sendConfirmation.Verify(s => s.ExecuteAsync(order.Id), Times.Once);
    }
}
