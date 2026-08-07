using MediatR;
using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Entities;
using VerticalSlice.Infrastructure.Persistence;

namespace VerticalSlice.Web.Features.Orders.UpdateOrder;

/// <summary>
/// The 10%-discount rule is recalculated here independently of
/// CreateOrderCommandHandler -- a deliberate duplication rather than a shared
/// helper, in keeping with each slice owning everything it needs.
/// </summary>
public class UpdateOrderCommandHandler(AppDbContext db) : IRequestHandler<UpdateOrderCommand, UpdateOrderResponse>
{
    private const decimal DiscountThreshold = 100m;
    private const decimal DiscountRate = 0.10m;

    public async Task<UpdateOrderResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException($"Order {request.OrderId} does not exist.");

        if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new InvalidOperationException($"Order {order.Id} cannot be modified because it is already {order.Status}.");

        order.CustomerId = request.CustomerId;
        order.Items.Clear();

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

        await db.SaveChangesAsync(cancellationToken);

        var discounted = order.Subtotal > DiscountThreshold ? order.Subtotal * (1 - DiscountRate) : order.Subtotal;

        return new UpdateOrderResponse(
            order.Id, order.CustomerId, order.Status.ToString(),
            order.Subtotal, order.Subtotal - discounted, discounted, order.CreatedAt);
    }
}
