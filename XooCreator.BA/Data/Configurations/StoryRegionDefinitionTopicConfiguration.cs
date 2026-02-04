using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryRegionDefinitionTopicConfiguration : IEntityTypeConfiguration<StoryRegionDefinitionTopic>
{
    public void Configure(EntityTypeBuilder<StoryRegionDefinitionTopic> builder)
    {
        builder.ToTable("StoryRegionDefinitionTopic", "alchimalia_schema");
        builder.HasKey(x => new { x.StoryRegionDefinitionId, x.StoryTopicId });
        builder.HasOne(x => x.StoryRegionDefinition).WithMany(x => x.Topics).HasForeignKey(x => x.StoryRegionDefinitionId);
        builder.HasOne(x => x.StoryTopic).WithMany().HasForeignKey(x => x.StoryTopicId);
    }
}
