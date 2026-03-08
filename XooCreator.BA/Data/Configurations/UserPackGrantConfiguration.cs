using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserPackGrantConfiguration : IEntityTypeConfiguration<UserPackGrant>
{
    public void Configure(EntityTypeBuilder<UserPackGrant> builder)
    {
        builder.ToTable("UserPackGrants", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.PlanId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.EmailUsed).HasMaxLength(255);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.GrantedAtUtc);
        builder.HasIndex(x => x.OrderId);
    }
}
