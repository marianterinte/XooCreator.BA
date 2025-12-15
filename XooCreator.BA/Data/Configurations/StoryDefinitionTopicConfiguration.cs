using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryDefinitionTopicConfiguration : IEntityTypeConfiguration<StoryDefinitionTopic>
{
    public void Configure(EntityTypeBuilder<StoryDefinitionTopic> builder)
    {
        builder.HasKey(x => new { x.StoryDefinitionId, x.StoryTopicId });
        builder.HasOne(x => x.StoryDefinition).WithMany(x => x.Topics).HasForeignKey(x => x.StoryDefinitionId);
        builder.HasOne(x => x.StoryTopic).WithMany(x => x.StoryDefinitions).HasForeignKey(x => x.StoryTopicId);
    }
}
