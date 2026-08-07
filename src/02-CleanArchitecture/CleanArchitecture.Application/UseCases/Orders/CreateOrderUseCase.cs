using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.UseCases.Orders;

public class CreateOrderUseCase(
    IValidateOrderUseCase validator,
    ICalculateOrderTotalUseCase totalCalculator,
    IProductLookup products,
    IAddOrder orderWriter) : ICreateOrderUseCase
{
    public async Task<OrderDto> ExecuteAsync(CreateOrderRequest request)
    {
        await validator.ValidateAsync(request);

        var items = new List<OrderItem>();
        foreach (var line in request.Items)
        {
            var product = await products.GetByIdAsync(line.ProductId)
                ?? throw new OrderDomainException($"Product {line.ProductId} does not exist.");

            items.Add(new OrderItem(product.Id, line.Quantity, new Money(product.UnitPrice)));
        }

        var order = Order.Create(request.CustomerId, items);
        await orderWriter.AddAsync(order);

        return OrderMapper.ToDto(order, totalCalculator);
    }
}
