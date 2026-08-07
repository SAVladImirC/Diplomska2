using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedOnAdd();

        builder.Property(o => o.Status).HasConversion<string>();

        builder.Ignore(o => o.Subtotal);
        builder.Ignore(o => o.DomainEvents);

        builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.OwnsMany(o => o.Items, items =>
        {
            items.WithOwner().HasForeignKey("OrderId");
            items.Property<int>("Id");
            items.HasKey("Id");

            items.Property(i => i.ProductId);
            items.Property(i => i.Quantity);
            items.Ignore(i => i.LineTotal);

            items.OwnsOne(i => i.UnitPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("UnitPriceAmount");
                money.Property(m => m.Currency).HasColumnName("UnitPriceCurrency");
            });
        });

        // Table-per-hierarchy: Order and PriorityOrder share one table, distinguished
        // by a discriminator column, so a repository built against Order transparently
        // reads/writes PriorityOrder rows too.
        builder.HasDiscriminator<string>("OrderType")
            .HasValue<Order>("Standard")
            .HasValue<PriorityOrder>("Priority");
    }
}
