using MediatR;

namespace VerticalSlice.Web.Features.Orders.DeleteOrder;

public static class DeleteOrderEndpoint
{
    public static void MapDeleteOrder(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/orders/{orderId:int}", async (int orderId, ISender sender) =>
        {
            await sender.Send(new DeleteOrderCommand(orderId));
            return Results.NoContent();
        });
    }
}
