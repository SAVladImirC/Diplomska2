using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Dtos;

public static class OrderMapper
{
    public static OrderDto ToDto(Order order, ICalculateOrderTotalUseCase totalCalculator)
    {
        var total = totalCalculator.Calculate(order.Subtotal);
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status.ToString(),
            Subtotal = order.Subtotal.Amount,
            DiscountApplied = order.Subtotal.Amount - total.Amount,
            Total = total.Amount,
            CreatedAt = order.CreatedAt
        };
    }
}
