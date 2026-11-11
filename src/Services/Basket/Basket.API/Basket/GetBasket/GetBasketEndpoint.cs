namespace Basket.API.Baskets.GetBasket;

public record GetBasketResponse(ShoppingCart Cart);

public class GetBasketEndpoint: ICarterModule
{
   public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket", async (ClaimsPrincipal user, ISender sender) =>
        {
            var result = await sender.Send(new GetBasketQuery(user.GetUserId()));
            var response = result.Adapt<GetBasketResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(EshopPolicies.Authenticated)
            .WithName("GetBasket")
            .Produces<GetBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Basket")
            .WithDescription("Get Basket");
    }
}
