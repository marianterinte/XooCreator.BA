using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicCraftTopicConfiguration : IEntityTypeConfiguration<StoryEpicCraftTopic>
{
    public void Configure(EntityTypeBuilder<StoryEpicCraftTopic> builder)
    {
        builder.HasKey(x => new { x.StoryEpicCraftId, x.StoryTopicId });
        builder.HasOne(x => x.StoryEpicCraft).WithMany(x => x.Topics).HasForeignKey(x => x.StoryEpicCraftId);
        builder.HasOne(x => x.StoryTopic).WithMany().HasForeignKey(x => x.StoryTopicId); // Unidirectional nav from topic usually sufficient, or update StoryTopic
    }
}
