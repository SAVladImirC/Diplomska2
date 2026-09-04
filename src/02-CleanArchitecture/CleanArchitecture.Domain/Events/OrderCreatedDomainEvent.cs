using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Events;

public record OrderCreatedDomainEvent(Order Order) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
