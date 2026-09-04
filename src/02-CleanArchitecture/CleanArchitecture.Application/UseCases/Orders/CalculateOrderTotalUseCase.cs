using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.UseCases.Orders;

public class CalculateOrderTotalUseCase : ICalculateOrderTotalUseCase
{
    private const decimal DiscountThreshold = 100m;
    private const decimal DiscountRate = 0.10m;

    public Money Calculate(Money subtotal) =>
        subtotal.Amount > DiscountThreshold ? subtotal * (1 - DiscountRate) : subtotal;
}
