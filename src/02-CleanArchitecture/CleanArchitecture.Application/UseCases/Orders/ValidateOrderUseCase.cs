using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Application.UseCases.Orders;

public class ValidateOrderUseCase(ICustomerLookup customers, IProductLookup products) : IValidateOrderUseCase
{
    public async Task ValidateAsync(CreateOrderRequest request)
    {
        if (request.Items.Count == 0)
            throw new OrderDomainException("An order must contain at least one item.");

        if (await customers.GetByIdAsync(request.CustomerId) is null)
            throw new OrderDomainException($"Customer {request.CustomerId} does not exist.");

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
                throw new OrderDomainException("Order item quantity must be positive.");

            if (await products.GetByIdAsync(item.ProductId) is null)
                throw new OrderDomainException($"Product {item.ProductId} does not exist.");
        }
    }
}
