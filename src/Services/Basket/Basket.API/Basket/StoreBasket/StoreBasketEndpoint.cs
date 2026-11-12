namespace Basket.API.Baskets.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart);
public record StoreBasketResponse(string UserName);

public class StoreBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async (StoreBasketRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new StoreBasketCommand(request.Cart, user.GetUserId());
            var result = await sender.Send(command);

            var response = result.Adapt<StoreBasketResponse>();

            return Results.Created("/basket", response);
        })
            .RequireAuthorization(EshopPolicies.Authenticated)
            .WithName("StoreBasket")
            .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Store Basket")
            .WithDescription("Store Basket");
    }
}
