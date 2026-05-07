using Ordering.Domain.Enums;

namespace Ordering.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasConversion(
            orderId => orderId.Value,
            dbId => OrderId.Of(dbId));

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .IsRequired();

        builder.HasMany(p => p.OrderItems)
            .WithOne()
            .HasForeignKey(p => p.OrderId);

        builder.ComplexProperty(p => p.OrderName, internalBuilder =>
        {
            internalBuilder.Property(i => i.Value)
            .HasColumnName(nameof(Order.OrderName))
            .HasMaxLength(100)
            .IsRequired();
        });

        builder.ComplexProperty(p => p.ShippingAddress, cb =>
        {
            cb.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            cb.Property(x => x.LastName).HasMaxLength(50).IsRequired();
            cb.Property(x => x.EmailAddress).HasMaxLength(50).IsRequired();
            cb.Property(x => x.AddressLine).HasMaxLength(180).IsRequired();
            cb.Property(x => x.Country).HasMaxLength(50).IsRequired();
            cb.Property(x => x.State).HasMaxLength(50).IsRequired();
            cb.Property(x => x.ZipCode).HasMaxLength(5).IsRequired();
        });

        builder.ComplexProperty(p => p.BillingAddress, cb =>
        {
            cb.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            cb.Property(x => x.LastName).HasMaxLength(50).IsRequired();
            cb.Property(x => x.EmailAddress).HasMaxLength(50).IsRequired();
            cb.Property(x => x.AddressLine).HasMaxLength(180).IsRequired();
            cb.Property(x => x.Country).HasMaxLength(50).IsRequired();
            cb.Property(x => x.State).HasMaxLength(50).IsRequired();
            cb.Property(x => x.ZipCode).HasMaxLength(5).IsRequired();
        });

        builder.ComplexProperty(p => p.Payment, cb =>
        {
            cb.Property(x => x.CardName).HasMaxLength(50).IsRequired();
            cb.Property(x => x.CardNumber).HasMaxLength(24).IsRequired();
            cb.Property(x => x.Expiration).HasMaxLength(10).IsRequired();
            cb.Property(x => x.CVV).HasMaxLength(3).IsRequired();
            cb.Property(x => x.PaymentMethod);
        });

        builder.Property(p => p.Status)
            .HasDefaultValue(OrderStatus.Draft)
            .HasConversion(
            x => x.ToString(),
            dbStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), dbStatus));
    }
}
