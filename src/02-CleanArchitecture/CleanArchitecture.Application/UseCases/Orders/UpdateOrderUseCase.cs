using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.UseCases.Orders;

public class UpdateOrderUseCase(
    IValidateOrderUseCase validator,
    ICalculateOrderTotalUseCase totalCalculator,
    IProductLookup products,
    IGetOrders orderReader,
    IUpdateOrder orderWriter,
    IUnitOfWork unitOfWork) : IUpdateOrderUseCase
{
    public async Task<OrderDto> ExecuteAsync(int orderId, CreateOrderRequest request)
    {
        var order = await orderReader.GetByIdAsync(orderId)
            ?? throw new OrderDomainException($"Order {orderId} does not exist.");

        await validator.ValidateAsync(request);

        var items = new List<OrderItem>();
        foreach (var line in request.Items)
        {
            var product = await products.GetByIdAsync(line.ProductId)
                ?? throw new OrderDomainException($"Product {line.ProductId} does not exist.");

            items.Add(new OrderItem(product.Id, line.Quantity, new Money(product.UnitPrice)));
        }

        order.UpdateDetails(request.CustomerId, items);
        await orderWriter.UpdateAsync(order);
        await unitOfWork.SaveChangesAsync();

        return OrderMapper.ToDto(order, totalCalculator);
    }
}
