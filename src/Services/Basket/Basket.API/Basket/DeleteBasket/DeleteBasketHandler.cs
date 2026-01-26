
namespace Basket.API.Baskets.DeleteBasket;

public record DeleteBasketCommand(string UserName): ICommand<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSuccess);

internal class DeleteBasketHandler(IDocumentSession session) :
    ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
