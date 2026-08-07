using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations;

public class PriorityOrderConfiguration : IEntityTypeConfiguration<PriorityOrder>
{
    public void Configure(EntityTypeBuilder<PriorityOrder> builder)
    {
        builder.Property(o => o.RequestedDeliveryDate);

        builder.OwnsOne(o => o.ExpediteFee, money =>
        {
            money.Property(m => m.Amount).HasColumnName("ExpediteFeeAmount");
            money.Property(m => m.Currency).HasColumnName("ExpediteFeeCurrency");
        });
    }
}
