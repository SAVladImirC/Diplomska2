using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.UseCases.Orders;

/// <summary>The recurring business rule across all three architectures: orders over 100 get a 10% discount.</summary>
public class CalculateOrderTotalUseCase : ICalculateOrderTotalUseCase
{
    private const decimal DiscountThreshold = 100m;
    private const decimal DiscountRate = 0.10m;

    public Money Calculate(Money subtotal) =>
        subtotal.Amount > DiscountThreshold ? subtotal * (1 - DiscountRate) : subtotal;
}
