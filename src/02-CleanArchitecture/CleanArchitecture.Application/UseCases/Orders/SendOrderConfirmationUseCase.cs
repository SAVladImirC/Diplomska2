using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Application.UseCases.Orders;

public class SendOrderConfirmationUseCase(IGetOrders orderReader, ICustomerLookup customers, IEmailService emailService)
    : ISendOrderConfirmationUseCase
{
    public async Task ExecuteAsync(int orderId)
    {
        var order = await orderReader.GetByIdAsync(orderId)
            ?? throw new OrderDomainException($"Order {orderId} does not exist.");
        var customer = await customers.GetByIdAsync(order.CustomerId)
            ?? throw new OrderDomainException($"Customer {order.CustomerId} does not exist.");

        await emailService.SendEmailAsync(
            customer.Email, "Order Confirmation", $"Your order #{order.Id} has been received.");
    }
}
