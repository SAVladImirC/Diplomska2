using MediatR;

namespace VerticalSlice.Web.Features.Orders.DeleteOrder;

public record DeleteOrderCommand(int OrderId) : IRequest;
