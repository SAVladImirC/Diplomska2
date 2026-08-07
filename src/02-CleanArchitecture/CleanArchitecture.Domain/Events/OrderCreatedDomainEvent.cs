namespace CleanArchitecture.Domain.Events;

public record OrderCreatedDomainEvent(int OrderId) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
