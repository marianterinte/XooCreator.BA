using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicDefinitionTopicConfiguration : IEntityTypeConfiguration<StoryEpicDefinitionTopic>
{
    public void Configure(EntityTypeBuilder<StoryEpicDefinitionTopic> builder)
    {
        builder.HasKey(x => new { x.StoryEpicDefinitionId, x.StoryTopicId });
        builder.HasOne(x => x.StoryEpicDefinition).WithMany(x => x.Topics).HasForeignKey(x => x.StoryEpicDefinitionId);
        builder.HasOne(x => x.StoryTopic).WithMany().HasForeignKey(x => x.StoryTopicId);
    }
}
