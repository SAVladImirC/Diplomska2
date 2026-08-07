using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.ValueObjects;
using Xunit;

namespace CleanArchitecture.Tests.UseCases;

/// <summary>The same recurring business rule as NTier.Tests.OrderServiceDiscountTests, in isolation here.</summary>
public class CalculateOrderTotalUseCaseTests
{
    private readonly CalculateOrderTotalUseCase _useCase = new();

    [Fact]
    public void Calculate_OverThreshold_Applies10PercentDiscount()
    {
        var total = _useCase.Calculate(new Money(120m));
        Assert.Equal(108m, total.Amount);
    }

    [Fact]
    public void Calculate_AtOrBelowThreshold_AppliesNoDiscount()
    {
        var total = _useCase.Calculate(new Money(100m));
        Assert.Equal(100m, total.Amount);
    }
}
