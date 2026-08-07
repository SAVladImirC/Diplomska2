using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.UseCases.Orders;

public interface ICreateOrderUseCase
{
    Task<OrderDto> ExecuteAsync(CreateOrderRequest request);
}
