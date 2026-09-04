using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.Abstractions;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent);
}
