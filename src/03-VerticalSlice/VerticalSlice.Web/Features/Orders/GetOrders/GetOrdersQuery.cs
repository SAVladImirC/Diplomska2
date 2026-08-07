using MediatR;

namespace VerticalSlice.Web.Features.Orders.GetOrders;

public record GetOrdersQuery : IRequest<List<OrderSummary>>;

public record OrderSummary(
    int Id,
    int CustomerId,
    string Status,
    decimal Subtotal,
    decimal DiscountApplied,
    decimal Total,
    DateTime CreatedAt);
