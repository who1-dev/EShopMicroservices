
using Ordering.Application.Orders.Commands.UpdateOrder;

namespace Ordering.API.Endpoints;

public record UpdateOrderRequest(OrderDto Order);

public record UpdateOrderResponse(bool IsSuccess);


public class UpdateOrder : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // :guid → a non-GUID id gives 404 instead of a binding error
        app.MapPut("/orders/{orderId:guid}", async (Guid orderId, UpdateOrderRequest request, ISender sender) =>
        {
            var command = new UpdateOrderCommand(orderId, request.Order);

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateOrderResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateOrder")
        .RequireAuthorization(EshopPolicies.Admin)
        .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Order")
        .WithDescription("Update Order");
    }
}
