using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.UseCases.Orders;

public interface IGetOrdersUseCase
{
    Task<List<OrderDto>> ExecuteAsync();
}
