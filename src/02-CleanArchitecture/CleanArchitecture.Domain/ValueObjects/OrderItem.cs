using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.ValueObjects;

/// <summary>Defined by its attributes (product, quantity, price), not by an identity.</summary>
public record OrderItem
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = Money.Zero;

    // EF Core cannot bind a constructor parameter to an owned-type reference like
    // Money, so materialization uses this parameterless constructor plus private
    // property setters instead of the public constructor below.
    private OrderItem()
    {
    }

    public OrderItem(int productId, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new OrderDomainException("Order item quantity must be positive.");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Money LineTotal => UnitPrice * Quantity;
}
