using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.EventHandlers;

public class OrderCreatedDomainEventHandler(ISendOrderConfirmationUseCase sendConfirmation)
    : IDomainEventHandler<OrderCreatedDomainEvent>
{
    public Task HandleAsync(OrderCreatedDomainEvent domainEvent) => sendConfirmation.ExecuteAsync(domainEvent.Order.Id);
}
