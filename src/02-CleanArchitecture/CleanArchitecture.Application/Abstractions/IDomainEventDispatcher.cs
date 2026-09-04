using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyList<IDomainEvent> domainEvents);
}
