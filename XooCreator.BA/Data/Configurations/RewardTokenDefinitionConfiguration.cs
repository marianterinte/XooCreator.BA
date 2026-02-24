using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class RewardTokenDefinitionConfiguration : IEntityTypeConfiguration<RewardTokenDefinition>
{
    public void Configure(EntityTypeBuilder<RewardTokenDefinition> builder)
    {
        builder.ToTable("RewardTokenDefinitions", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(128).IsRequired();
        builder.Property(x => x.DisplayNameKey).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Icon).HasMaxLength(32);
        builder.HasIndex(x => new { x.Type, x.Value }).IsUnique();
    }
}
