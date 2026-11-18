using BuildingBlocks.Contracts.Enums;

namespace Ordering.Domain.ValueObjects;

// No card data is stored (PCI DSS): only the payment method and the payment provider's reference.
public record Payment
{
    public PaymentMethod PaymentMethod { get; } = default!;
    public string Reference { get; } = default!;

    protected Payment()
    {
    }

    private Payment(PaymentMethod paymentMethod, string reference)
    {
        PaymentMethod = paymentMethod;
        Reference = reference;
    }

    public static Payment Of(PaymentMethod paymentMethod, string reference)
    {
        if (!Enum.IsDefined(paymentMethod))
            throw new ArgumentOutOfRangeException(nameof(paymentMethod), paymentMethod, "Unknown payment method");
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(reference.Length, 64);

        return new Payment(paymentMethod, reference);
    }
}