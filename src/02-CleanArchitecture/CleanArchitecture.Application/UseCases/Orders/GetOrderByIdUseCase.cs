using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.UseCases.Orders;

public class GetOrderByIdUseCase(IOrderQueries queries, ICalculateOrderTotalUseCase totalCalculator) : IGetOrderByIdUseCase
{
    public async Task<OrderDto?> ExecuteAsync(int orderId)
    {
        var order = await queries.GetByIdAsync(orderId);
        return order is null ? null : OrderMapper.ToDto(order, totalCalculator);
    }
}
