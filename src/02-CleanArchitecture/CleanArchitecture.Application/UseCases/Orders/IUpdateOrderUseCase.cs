using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.UseCases.Orders;

public interface IUpdateOrderUseCase
{
    Task<OrderDto> ExecuteAsync(int orderId, CreateOrderRequest request);
}
