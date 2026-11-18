
namespace Ordering.Application.Orders.Commands.CreateOrder;

internal class CreateOrderHandler(IApplicationDbContext dbContext)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Order;
        var customerId = CustomerId.Of(dto.CustomerId);

        // First order from this Keycloak user → create the Customer from the checkout details
        if (!await dbContext.Customers.AnyAsync(c => c.Id == customerId, cancellationToken))
        {
            var name = $"{dto.ShippingAddress.FirstName} {dto.ShippingAddress.LastName}";
            dbContext.Customers.Add(Customer.Create(customerId, name, dto.ShippingAddress.EmailAddress));
        }

        var order = CreateNewOrder(dto);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);   // customer + order in one transaction

        return new CreateOrderResult(order.Id.Value);
    }

    private Order CreateNewOrder(OrderDto orderDto)
    {
        var shippingAddress = Address.Of(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var billingAddress = Address.Of(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);

        var newOrder = Order.Create(
                id: OrderId.Of(Guid.NewGuid()),
                customerId: CustomerId.Of(orderDto.CustomerId),
                orderName: OrderName.Of(orderDto.OrderName),
                shippingAddress: shippingAddress,
                billingAddress: billingAddress,
                // No real payment provider: the reference is generated here, never taken from the client
                payment: Payment.Of(orderDto.Payment.PaymentMethod, NewSimulatedReference())
                );

        foreach (var orderItemDto in orderDto.OrderItems)
        {
            newOrder.Add(ProductId.Of(orderItemDto.ProductId), orderItemDto.Quantity, orderItemDto.Price);
        }
        return newOrder;
    }

    // e.g. SIM-3f9c2a7b1d4e (a real provider would return its own id here, like Stripe's pi_...)
    private static string NewSimulatedReference() => $"SIM-{Guid.NewGuid():N}"[..16];
}
