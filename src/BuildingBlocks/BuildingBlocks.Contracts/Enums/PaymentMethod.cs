namespace BuildingBlocks.Contracts.Enums;

// How the customer chose to pay. Only the choice is stored: card details would go to a payment
// provider (e.g. Stripe), never to this system.
// Sent as a number in JSON and RabbitMQ messages and stored as an int in OrderDb:
// never renumber or remove a value, only add new ones at the end.
public enum PaymentMethod
{
    CreditCard = 1,
    DebitCard = 2,
    PayPal = 3,
    CashOnDelivery = 4
}
