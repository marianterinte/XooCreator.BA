using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class TreeStoryNodeConfiguration : IEntityTypeConfiguration<TreeStoryNode>
{
    public void Configure(EntityTypeBuilder<TreeStoryNode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RegionId).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.StoryId, x.RegionId, x.TreeConfigurationId }).IsUnique();
        builder.HasOne(x => x.Region).WithMany(x => x.Stories).HasForeignKey(x => x.RegionId);
        builder.HasOne(x => x.StoryDefinition).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId);
        builder.HasOne(x => x.TreeConfiguration).WithMany().HasForeignKey(x => x.TreeConfigurationId);
    }
}
