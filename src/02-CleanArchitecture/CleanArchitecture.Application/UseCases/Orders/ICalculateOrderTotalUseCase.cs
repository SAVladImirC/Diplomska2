using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.UseCases.Orders;

public interface ICalculateOrderTotalUseCase
{
    Money Calculate(Money subtotal);
}
