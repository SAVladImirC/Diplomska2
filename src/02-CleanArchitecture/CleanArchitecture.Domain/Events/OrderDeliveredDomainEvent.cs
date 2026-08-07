namespace CleanArchitecture.Domain.Events;

public record OrderDeliveredDomainEvent(int OrderId) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
