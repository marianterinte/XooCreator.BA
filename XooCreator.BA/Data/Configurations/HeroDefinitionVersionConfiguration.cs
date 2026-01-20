using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroDefinitionVersionConfiguration : IEntityTypeConfiguration<HeroDefinitionVersion>
{
    public void Configure(EntityTypeBuilder<HeroDefinitionVersion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.HeroDefinitionId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.HeroDefinitionId, x.Version }).IsUnique();
        builder.HasOne(x => x.HeroDefinition)
            .WithMany()
            .HasForeignKey(x => x.HeroDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
