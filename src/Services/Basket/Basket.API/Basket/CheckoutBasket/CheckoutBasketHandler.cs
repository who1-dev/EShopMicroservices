
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto, string UserId, Guid CustomerId)
    : ICommand<CheckoutBasketResult>;
public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator
    : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckoutDto).NotNull().WithMessage("BasketCheckoutDto can't be null");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required");

        // Validate before the basket is deleted: if Ordering rejected these later in the
        // consumer, the user would lose the basket and get no order and no error
        When(x => x.BasketCheckoutDto is not null, () =>
        {
            RuleFor(x => x.BasketCheckoutDto.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(x => x.BasketCheckoutDto.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.BasketCheckoutDto.EmailAddress).NotEmpty().EmailAddress();
        });
    }
}

public class CheckoutBasketHandler
    (IBasketRepository repository, IPublishEndpoint publishEndpoint)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        // get existing basket with total price
        // Set totalprice on basketcheckout event message
        // send basket checkout event to rabbitmq using masstransit
        // delete the basket

        var basket = await repository.GetBasket(command.UserId, cancellationToken);
        if (basket == null)
        {
            return new CheckoutBasketResult(false);
        }

        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.UserName = command.UserId;        // identity comes from the token,
        eventMessage.CustomerId = command.CustomerId;  // not from the DTO
        eventMessage.TotalPrice = basket.TotalPrice;

        await publishEndpoint.Publish(eventMessage, cancellationToken);

        await repository.DeleteBasket(command.UserId, cancellationToken);

        return new CheckoutBasketResult(true);
    }
}