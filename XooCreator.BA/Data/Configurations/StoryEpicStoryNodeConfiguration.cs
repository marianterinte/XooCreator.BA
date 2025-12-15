using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicStoryNodeConfiguration : IEntityTypeConfiguration<StoryEpicStoryNode>
{
    public void Configure(EntityTypeBuilder<StoryEpicStoryNode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RegionId).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.EpicId, x.StoryId, x.RegionId }).IsUnique();
        builder.HasOne(x => x.Epic).WithMany(x => x.StoryNodes).HasForeignKey(x => x.EpicId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Region).WithMany(x => x.Stories).HasForeignKey(x => new { x.EpicId, x.RegionId }).HasPrincipalKey(r => new { r.EpicId, r.RegionId }).OnDelete(DeleteBehavior.Cascade);
        // StoryCraft and StoryDefinition are navigation properties only (no FK constraints in DB)
        // Application code will determine which one exists based on StoryId
        // We use HasPrincipalKey to map to StoryId but don't create FK constraints
        builder.HasOne(x => x.StoryCraft).WithMany().HasPrincipalKey(s => s.StoryId).HasForeignKey(x => x.StoryId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
        builder.HasOne(x => x.StoryDefinition).WithMany().HasPrincipalKey(s => s.StoryId).HasForeignKey(x => x.StoryId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
    }
}
