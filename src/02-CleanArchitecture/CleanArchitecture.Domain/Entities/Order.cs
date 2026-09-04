using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

public record Order
{
    public int Id { get; private set; }
    public int CustomerId { get; protected set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsDeleted { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items;

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected Order()
    {
    }

    public static Order Create(int customerId, IReadOnlyList<OrderItem> items)
    {
        EnsureHasItems(items);

        var order = new Order { CustomerId = customerId };
        order.SetItems(items);
        order._domainEvents.Add(new OrderCreatedDomainEvent(order));
        return order;
    }

    protected static void EnsureHasItems(IReadOnlyList<OrderItem> items)
    {
        if (items.Count == 0)
            throw new OrderDomainException("An order must contain at least one item.");
    }

    protected void SetItems(IEnumerable<OrderItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }

    public Money Subtotal => Items.Aggregate(Money.Zero, (total, item) => total + item.LineTotal);

    public void UpdateDetails(int customerId, IReadOnlyList<OrderItem> items)
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new OrderDomainException($"Order {Id} cannot be modified because it is already {Status}.");

        EnsureHasItems(items);
        CustomerId = customerId;
        SetItems(items);
    }

    public void MarkDelivered()
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new OrderDomainException($"Order {Id} cannot transition from {Status} to Delivered.");

        Status = OrderStatus.Delivered;
        _domainEvents.Add(new OrderDeliveredDomainEvent(this));
    }

    public void MarkDeleted() => IsDeleted = true;

    public void ClearDomainEvents() => _domainEvents.Clear();

    public virtual bool Equals(Order? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id != 0 && Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}
