using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicDefinitionStoryNodeConfiguration : IEntityTypeConfiguration<StoryEpicDefinitionStoryNode>
{
    public void Configure(EntityTypeBuilder<StoryEpicDefinitionStoryNode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RegionId).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.EpicId, x.StoryId, x.RegionId }).IsUnique();
        builder.HasOne(x => x.Epic).WithMany(x => x.StoryNodes).HasForeignKey(x => x.EpicId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Region).WithMany(x => x.Stories).HasForeignKey(x => new { x.EpicId, x.RegionId }).HasPrincipalKey(r => new { r.EpicId, r.RegionId }).OnDelete(DeleteBehavior.Cascade);
        // Only StoryDefinition for published epics
        builder.HasOne(x => x.StoryDefinition).WithMany().HasPrincipalKey(s => s.StoryId).HasForeignKey(x => x.StoryId).OnDelete(DeleteBehavior.Restrict).IsRequired();
    }
}
