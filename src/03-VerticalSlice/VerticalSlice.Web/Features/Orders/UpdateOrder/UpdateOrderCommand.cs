using MediatR;

namespace VerticalSlice.Web.Features.Orders.UpdateOrder;

public record UpdateOrderCommand(int OrderId, int CustomerId, List<UpdateOrderItem> Items) : IRequest<UpdateOrderResponse>;

public record UpdateOrderItem(int ProductId, int Quantity);

public record UpdateOrderResponse(
    int Id,
    int CustomerId,
    string Status,
    decimal Subtotal,
    decimal DiscountApplied,
    decimal Total,
    DateTime CreatedAt);
