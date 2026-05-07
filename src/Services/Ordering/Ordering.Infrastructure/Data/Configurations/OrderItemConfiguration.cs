namespace Ordering.Infrastructure.Data.Configurations;

public class OrderItemConfiguration: IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id).HasConversion(
            orderItemId => orderItemId.Value,
            dbId => OrderItemId.Of(dbId));

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(p => p.ProductId);

        builder.Property(p => p.Quantity).IsRequired();

        builder.Property(p => p.Price).IsRequired();
    }
}
