
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints;


public record GetOrdersByCustomerResponse(IEnumerable<OrderDto> Orders);

public class GetOrdersByCustomer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/me", async (ClaimsPrincipal user, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersByCustomerQuery(user.GetUserGuid()));

            var response = result.Adapt<GetOrdersByCustomerResponse>();

            return Results.Ok(response);
        })
            .WithName("GetOrdersByCustomer")
            .RequireAuthorization(EshopPolicies.Authenticated)
            .Produces<GetOrdersByCustomerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get Orders By Customer")
            .WithDescription("Get Orders By Customer");

    }
}
