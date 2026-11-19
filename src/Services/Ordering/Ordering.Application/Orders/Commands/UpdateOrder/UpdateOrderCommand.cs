using FluentValidation;

namespace Ordering.Application.Orders.Commands.UpdateOrder;

// OrderId comes from the URL (PUT /orders/{orderId}); Order.Id from the body must match it
public record UpdateOrderCommand(Guid OrderId, OrderDto Order) : ICommand<UpdateOrderResult>;

public record UpdateOrderResult(bool IsSuccess);

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderId is required");
        RuleFor(x => x.Order).NotNull().WithMessage("Order is required");

        When(x => x.Order is not null, () =>
        {
            // Without this, PUT /orders/A with B's id in the body would update B
            RuleFor(x => x.Order.Id).Equal(x => x.OrderId)
                .WithMessage("Order id in the body must match the order id in the URL");
            RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("CustomerId is required");

            // The handler rebuilds both addresses with Address.Of, which throws (500) on these
            RuleFor(x => x.Order.ShippingAddress).NotNull().WithMessage("ShippingAddress is required");
            When(x => x.Order.ShippingAddress is not null, () =>
            {
                RuleFor(x => x.Order.ShippingAddress.EmailAddress).NotEmpty().EmailAddress();
                RuleFor(x => x.Order.ShippingAddress.AddressLine).NotEmpty().WithMessage("ShippingAddress.AddressLine is required");
            });
            RuleFor(x => x.Order.BillingAddress).NotNull().WithMessage("BillingAddress is required");
            When(x => x.Order.BillingAddress is not null, () =>
            {
                RuleFor(x => x.Order.BillingAddress.EmailAddress).NotEmpty().EmailAddress();
                RuleFor(x => x.Order.BillingAddress.AddressLine).NotEmpty().WithMessage("BillingAddress.AddressLine is required");
            });
        });
    }
}
