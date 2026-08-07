using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;
using Xunit;

namespace CleanArchitecture.Tests.Domain;

public class OrderTests
{
    private static OrderItem OneItem() => new(productId: 1, quantity: 2, unitPrice: new Money(60m));

    [Fact]
    public void Create_WithNoItems_Throws()
    {
        Assert.Throws<OrderDomainException>(() => Order.Create(customerId: 1, items: []));
    }

    [Fact]
    public void UpdateDetails_OnDeliveredOrder_ThrowsAsDocumentedBusinessRule()
    {
        var order = Order.Create(1, [OneItem()]);
        order.MarkDelivered();

        Assert.Throws<OrderDomainException>(() => order.UpdateDetails(1, [OneItem()]));
    }

    [Fact]
    public void MarkDeleted_OnDeliveredOrder_NeverThrows()
    {
        // The same "delivered" scenario that breaks NTier's IRepository<Order>.DeleteAsync
        // (see OrderRepositoryLspTests in NTier.Tests) succeeds here without incident,
        // because ISoftDeleteOrder only ever promises a soft delete.
        var order = Order.Create(1, [OneItem()]);
        order.MarkDelivered();

        var exception = Record.Exception(order.MarkDeleted);

        Assert.Null(exception);
        Assert.True(order.IsDeleted);
    }

    [Fact]
    public void PriorityOrder_SubstitutesForOrder_WithoutSurprises()
    {
        // A PriorityOrder can be used anywhere an Order is expected: no throwing
        // repository method, no NotImplementedException, no special-casing needed.
        Order order = PriorityOrder.Create(
            customerId: 1,
            items: [OneItem()],
            requestedDeliveryDate: DateTime.UtcNow.AddDays(1),
            expediteFee: new Money(15m));

        Assert.Equal(120m, order.Subtotal.Amount);
        order.MarkDelivered();
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void Equality_IsByIdentity_NotByStructure()
    {
        // Order is an Entity, not a Value Object: two distinct in-memory instances
        // with identical field values but no assigned Id are not the same order.
        var first = Order.Create(1, [OneItem()]);
        var second = Order.Create(1, [OneItem()]);

        Assert.NotEqual(first, second);
    }
}
