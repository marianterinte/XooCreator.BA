using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicRegionReferenceConfiguration : IEntityTypeConfiguration<StoryEpicRegionReference>
{
    public void Configure(EntityTypeBuilder<StoryEpicRegionReference> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RegionId).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.EpicId, x.RegionId }).IsUnique();
        builder.HasOne(x => x.Epic).WithMany().HasForeignKey(x => x.EpicId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Region).WithMany(x => x.EpicReferences).HasForeignKey(x => x.RegionId).OnDelete(DeleteBehavior.Restrict); // Prevent deletion if used in epics
    }
}
