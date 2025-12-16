using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroDefinitionConfiguration : IEntityTypeConfiguration<HeroDefinition>
{
    public void Configure(EntityTypeBuilder<HeroDefinition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(100);
        builder.Property(x => x.Type).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PrerequisitesJson).HasMaxLength(2000);
        builder.Property(x => x.RewardsJson).HasMaxLength(2000);
        builder.Property(x => x.PositionX).HasColumnType("decimal(10,6)");
        builder.Property(x => x.PositionY).HasColumnType("decimal(10,6)");
        builder.HasIndex(x => x.Id).IsUnique();
    }
}
