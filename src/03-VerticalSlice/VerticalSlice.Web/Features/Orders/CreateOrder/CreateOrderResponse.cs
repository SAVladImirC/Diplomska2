namespace VerticalSlice.Web.Features.Orders.CreateOrder;

public record CreateOrderResponse(
    int Id,
    int CustomerId,
    string Status,
    decimal Subtotal,
    decimal DiscountApplied,
    decimal Total,
    DateTime CreatedAt);
