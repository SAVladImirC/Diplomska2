using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.ValueObjects;

public record OrderItem
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = Money.Zero;

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
