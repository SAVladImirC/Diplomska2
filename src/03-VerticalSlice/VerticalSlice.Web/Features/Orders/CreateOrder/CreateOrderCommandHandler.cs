using MediatR;
using VerticalSlice.Infrastructure.Entities;
using VerticalSlice.Infrastructure.Persistence;
using VerticalSlice.Infrastructure.Services;

namespace VerticalSlice.Web.Features.Orders.CreateOrder;

/// <summary>
/// Talks to AppDbContext directly rather than through a repository abstraction --
/// the common practice for vertical slices described in the thesis. The 10%-discount
/// rule is calculated here and, deliberately, again in UpdateOrderCommandHandler
/// rather than in a shared helper, to keep the two slices independent at the cost of
/// duplication.
/// </summary>
public class CreateOrderCommandHandler(AppDbContext db, IEmailService emailService)
    : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private const decimal DiscountThreshold = 100m;
    private const decimal DiscountRate = 0.10m;

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await db.Customers.FindAsync([request.CustomerId], cancellationToken)
            ?? throw new InvalidOperationException($"Customer {request.CustomerId} does not exist.");

        var order = new Order { CustomerId = request.CustomerId, Status = OrderStatus.Pending };

        foreach (var line in request.Items)
        {
            var product = await db.Products.FindAsync([line.ProductId], cancellationToken)
                ?? throw new InvalidOperationException($"Product {line.ProductId} does not exist.");

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = product.UnitPrice
            });
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);

        await emailService.SendEmailAsync(
            customer.Email, "Order Confirmation", $"Your order #{order.Id} has been received.");

        var discounted = order.Subtotal > DiscountThreshold ? order.Subtotal * (1 - DiscountRate) : order.Subtotal;

        return new CreateOrderResponse(
            order.Id, order.CustomerId, order.Status.ToString(),
            order.Subtotal, order.Subtotal - discounted, discounted, order.CreatedAt);
    }
}
