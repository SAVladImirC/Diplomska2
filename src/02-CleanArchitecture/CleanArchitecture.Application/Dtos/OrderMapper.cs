using CleanArchitecture.Application.ReadModels;
using CleanArchitecture.Application.UseCases.Orders;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.Dtos;

public static class OrderMapper
{
    public static OrderDto ToDto(Order order, ICalculateOrderTotalUseCase totalCalculator) =>
        ToDto(order.Id, order.CustomerId, order.Status, order.Subtotal, order.CreatedAt, totalCalculator);

    public static OrderDto ToDto(OrderReadModel order, ICalculateOrderTotalUseCase totalCalculator) =>
        ToDto(order.Id, order.CustomerId, order.Status, new Money(order.Subtotal), order.CreatedAt, totalCalculator);

    private static OrderDto ToDto(
        int id, int customerId, OrderStatus status, Money subtotal, DateTime createdAt,
        ICalculateOrderTotalUseCase totalCalculator)
    {
        var total = totalCalculator.Calculate(subtotal);
        return new OrderDto
        {
            Id = id,
            CustomerId = customerId,
            Status = status.ToString(),
            Subtotal = subtotal.Amount,
            DiscountApplied = subtotal.Amount - total.Amount,
            Total = total.Amount,
            CreatedAt = createdAt
        };
    }
}
