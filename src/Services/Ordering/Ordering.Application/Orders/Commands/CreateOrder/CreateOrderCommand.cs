using FluentValidation;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(OrderDto Order) : ICommand<CreateOrderResult>;
public record CreateOrderResult(Guid Id);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Order).NotNull().WithMessage("Order is required");

        When(x => x.Order is not null, () =>
        {
            RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("CustomerId is required");
            RuleFor(x => x.Order.OrderItems).NotEmpty().WithMessage("OrderItems should not be empty");

            // Needed by the auto-created Customer: Customer.Create throws on blanks → 500
            RuleFor(x => x.Order.ShippingAddress).NotNull().WithMessage("ShippingAddress is required");
            When(x => x.Order.ShippingAddress is not null, () =>
            {
                RuleFor(x => x.Order.ShippingAddress.FirstName).NotEmpty().WithMessage("FirstName is required");
                RuleFor(x => x.Order.ShippingAddress.LastName).NotEmpty().WithMessage("LastName is required");
                RuleFor(x => x.Order.ShippingAddress.EmailAddress).NotEmpty().EmailAddress();
            });
        });
    }
}