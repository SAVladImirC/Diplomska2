using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

public sealed record PriorityOrder : Order
{
    public DateTime RequestedDeliveryDate { get; private set; }

    public Money? ExpediteFee { get; private set; }

    private PriorityOrder()
    {
    }

    public static PriorityOrder Create(
        int customerId,
        IReadOnlyList<OrderItem> items,
        DateTime requestedDeliveryDate,
        Money expediteFee)
    {
        EnsureHasItems(items);
        if (requestedDeliveryDate <= DateTime.UtcNow)
            throw new OrderDomainException("Requested delivery date must be in the future.");

        var order = new PriorityOrder
        {
            CustomerId = customerId,
            RequestedDeliveryDate = requestedDeliveryDate,
            ExpediteFee = expediteFee
        };
        order.SetItems(items);
        return order;
    }
}
