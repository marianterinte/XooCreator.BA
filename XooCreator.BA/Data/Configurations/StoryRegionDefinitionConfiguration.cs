using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryRegionDefinitionConfiguration : IEntityTypeConfiguration<StoryRegionDefinition>
{
    public void Configure(EntityTypeBuilder<StoryRegionDefinition> builder)
    {
        builder.ToTable("StoryRegionDefinitions", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => new { x.OwnerUserId, x.Id }).IsUnique();
        builder.HasIndex(x => new { x.Id, x.Version });
        builder.HasIndex(x => x.Status);
        builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Cascade);
    }
}

