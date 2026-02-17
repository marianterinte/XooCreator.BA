using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.PlanId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.StripeSessionId).HasMaxLength(255);
        builder.Property(x => x.StripeCustomerId).HasMaxLength(255);
        builder.Property(x => x.StripePaymentIntentId).HasMaxLength(255);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.StripeSessionId);
        builder.HasIndex(x => x.ExpiresAtUtc);
    }
}
