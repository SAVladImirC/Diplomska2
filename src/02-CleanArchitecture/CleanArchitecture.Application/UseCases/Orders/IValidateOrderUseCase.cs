using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.UseCases.Orders;

public interface IValidateOrderUseCase
{
    Task ValidateAsync(CreateOrderRequest request);
}
