
namespace Basket.API.Baskets.StoreBasket;

public record StoreBasketCommand(string UserName, ShoppingCart Cart) : ICommand<StoreBastetResult>
public record StoreBastetResult(Guid Id);

internal class StoreBasketHandler(IDocumentSession session) : ICommandHandler<StoreBasketCommand, StoreBastetResult>
{
    public async Task<StoreBastetResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
