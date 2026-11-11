namespace Basket.API.Baskets.DeleteBasket;

public record DeleteBasketResponse(bool IsSuccess);

public class DeleteBasketEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket", async (ClaimsPrincipal user, ISender sender) =>
            {
                var result = await sender.Send(new DeleteBasketCommand(user.GetUserId()));
                var response = result.Adapt<DeleteBasketResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization(EshopPolicies.Authenticated)
            .WithName("DeleteBasket")
            .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Basket")
            .WithDescription("Delete Basket");
    }
}
