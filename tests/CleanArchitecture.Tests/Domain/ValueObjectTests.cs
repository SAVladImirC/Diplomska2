using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;
using Xunit;

namespace CleanArchitecture.Tests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void Money_WithEqualAmountAndCurrency_AreEqual()
    {
        Assert.Equal(new Money(50m, "MKD"), new Money(50m, "MKD"));
    }

    [Fact]
    public void Money_NegativeAmount_Throws()
    {
        Assert.Throws<OrderDomainException>(() => new Money(-1m));
    }

    [Fact]
    public void OrderItem_NonPositiveQuantity_Throws()
    {
        Assert.Throws<OrderDomainException>(() => new OrderItem(1, 0, Money.Zero));
    }

    [Fact]
    public void OrderItem_LineTotal_MultipliesUnitPriceByQuantity()
    {
        var item = new OrderItem(1, 3, new Money(10m));
        Assert.Equal(30m, item.LineTotal.Amount);
    }
}
