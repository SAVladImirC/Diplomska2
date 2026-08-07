using MediatR;

namespace VerticalSlice.Web.Features.Orders.CreateOrder;

public record CreateOrderCommand(int CustomerId, List<CreateOrderItem> Items) : IRequest<CreateOrderResponse>;

public record CreateOrderItem(int ProductId, int Quantity);
