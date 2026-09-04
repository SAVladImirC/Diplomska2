using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.UseCases.Orders;

public class GetOrdersUseCase(IOrderQueries queries, ICalculateOrderTotalUseCase totalCalculator) : IGetOrdersUseCase
{
    public async Task<List<OrderDto>> ExecuteAsync()
    {
        var orders = await queries.GetAllAsync();
        return orders.Select(o => OrderMapper.ToDto(o, totalCalculator)).ToList();
    }
}
