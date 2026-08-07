using MediatR;

namespace VerticalSlice.Web.Features.Orders.GetOrders;

public static class GetOrdersEndpoint
{
    public static void MapGetOrders(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async (ISender sender) => Results.Ok(await sender.Send(new GetOrdersQuery())));
    }
}
