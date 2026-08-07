using FluentValidation;
using MediatR;
using VerticalSlice.Web.Common;

namespace VerticalSlice.Web.Features.Orders.CreateOrder;

public static class CreateOrderEndpoint
{
    public static void MapCreateOrder(this IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (
            CreateOrderCommand command,
            IValidator<CreateOrderCommand> validator,
            ISender sender) =>
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToErrorDictionary());

            var response = await sender.Send(command);
            return Results.Ok(response);
        });
    }
}
