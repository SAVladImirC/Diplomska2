using MediatR;
using Microsoft.EntityFrameworkCore;
using VerticalSlice.Infrastructure.Persistence;

namespace VerticalSlice.Web.Features.Orders.GetOrders;

public class GetOrdersQueryHandler(AppDbContext db) : IRequestHandler<GetOrdersQuery, List<OrderSummary>>
{
    private const decimal DiscountThreshold = 100m;
    private const decimal DiscountRate = 0.10m;

    public async Task<List<OrderSummary>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await db.Orders
            .Include(o => o.Items)
            .Where(o => !o.IsDeleted)
            .ToListAsync(cancellationToken);

        return orders.Select(o =>
        {
            var discounted = o.Subtotal > DiscountThreshold ? o.Subtotal * (1 - DiscountRate) : o.Subtotal;
            return new OrderSummary(
                o.Id, o.CustomerId, o.Status.ToString(), o.Subtotal, o.Subtotal - discounted, discounted, o.CreatedAt);
        }).ToList();
    }
}
