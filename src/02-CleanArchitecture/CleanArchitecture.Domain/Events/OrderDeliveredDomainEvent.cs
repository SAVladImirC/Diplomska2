using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Events;

public record OrderDeliveredDomainEvent(Order Order) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
