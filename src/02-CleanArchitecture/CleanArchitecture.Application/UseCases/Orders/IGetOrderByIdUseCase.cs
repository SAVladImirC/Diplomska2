using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.UseCases.Orders;

public interface IGetOrderByIdUseCase
{
    Task<OrderDto?> ExecuteAsync(int orderId);
}
