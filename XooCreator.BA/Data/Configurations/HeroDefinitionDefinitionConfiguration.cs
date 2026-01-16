using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroDefinitionDefinitionConfiguration : IEntityTypeConfiguration<HeroDefinitionDefinition>
{
    public void Configure(EntityTypeBuilder<HeroDefinitionDefinition> builder)
    {
        builder.ToTable("HeroDefinitionDefinitions", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Image).HasMaxLength(500);
        builder.HasIndex(x => x.Status);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.PublishedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
