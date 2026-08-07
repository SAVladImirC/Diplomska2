using FluentValidation;
using MediatR;
using VerticalSlice.Web.Common;

namespace VerticalSlice.Web.Features.Orders.UpdateOrder;

public static class UpdateOrderEndpoint
{
    public static void MapUpdateOrder(this IEndpointRouteBuilder app)
    {
        app.MapPut("/orders/{orderId:int}", async (
            int orderId,
            UpdateOrderRequestBody body,
            IValidator<UpdateOrderCommand> validator,
            ISender sender) =>
        {
            var command = new UpdateOrderCommand(orderId, body.CustomerId, body.Items);

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToErrorDictionary());

            var response = await sender.Send(command);
            return Results.Ok(response);
        });
    }
}

public record UpdateOrderRequestBody(int CustomerId, List<UpdateOrderItem> Items);
