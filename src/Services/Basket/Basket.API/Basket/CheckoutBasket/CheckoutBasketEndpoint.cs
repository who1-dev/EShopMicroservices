namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckoutDto);
public record CheckoutBasketResponse(bool IsSuccess);

public class CheckoutBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new CheckoutBasketCommand(request.BasketCheckoutDto, user.GetUserId(), user.GetUserGuid());
            var result = await sender.Send(command);

            var response = result.Adapt<CheckoutBasketResponse>();

            return Results.Ok(response);
        })
        .RequireAuthorization(EshopPolicies.Authenticated)
        .WithName("CheckoutBasket")
        .Produces<CheckoutBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Checkout Basket")
        .WithDescription("Checkout Basket");
    }
}