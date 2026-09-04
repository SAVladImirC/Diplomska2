using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
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
    public void Create_RaisesOrderCreatedDomainEvent()
    {
        var order = Order.Create(1, [OneItem()]);

        var created = Assert.IsType<OrderCreatedDomainEvent>(Assert.Single(order.DomainEvents));
        Assert.Same(order, created.Order);
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
        var order = Order.Create(1, [OneItem()]);
        order.MarkDelivered();

        var exception = Record.Exception(order.MarkDeleted);

        Assert.Null(exception);
        Assert.True(order.IsDeleted);
    }

    [Fact]
    public void PriorityOrder_SubstitutesForOrder_WithoutSurprises()
    {
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
        var first = Order.Create(1, [OneItem()]);
        var second = Order.Create(1, [OneItem()]);

        Assert.NotEqual(first, second);
    }
}
