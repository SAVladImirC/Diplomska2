using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// An expedited order. Because Clean Architecture favours small, role-based
/// interfaces (see <c>IAddOrder</c>, <c>ISoftDeleteOrder</c>) over the generic
/// <c>IRepository&lt;T&gt;</c> the N-Tier layer uses, a PriorityOrder substitutes
/// safely anywhere an Order is expected -- there is no throwing repository method
/// waiting to be surprised by this subtype.
/// </summary>
public sealed record PriorityOrder : Order
{
    public DateTime RequestedDeliveryDate { get; private set; }

    // Nullable so EF Core's TPH mapping can leave this owned value entirely absent
    // for plain Order rows in the shared table, instead of trying to construct a
    // Money from all-null columns (which Money's validating constructor rejects).
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
