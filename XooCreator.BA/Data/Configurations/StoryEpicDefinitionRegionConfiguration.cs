using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicDefinitionRegionConfiguration : IEntityTypeConfiguration<StoryEpicDefinitionRegion>
{
    public void Configure(EntityTypeBuilder<StoryEpicDefinitionRegion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RegionId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Label).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.EpicId, x.RegionId }).IsUnique();
        builder.HasOne(x => x.Epic).WithMany(x => x.Regions).HasForeignKey(x => x.EpicId).OnDelete(DeleteBehavior.Cascade);
    }
}
