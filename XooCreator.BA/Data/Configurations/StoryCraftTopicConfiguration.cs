using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftTopicConfiguration : IEntityTypeConfiguration<StoryCraftTopic>
{
    public void Configure(EntityTypeBuilder<StoryCraftTopic> builder)
    {
        builder.HasKey(x => new { x.StoryCraftId, x.StoryTopicId });
        builder.HasOne(x => x.StoryCraft).WithMany(x => x.Topics).HasForeignKey(x => x.StoryCraftId);
        builder.HasOne(x => x.StoryTopic).WithMany(x => x.StoryCrafts).HasForeignKey(x => x.StoryTopicId);
    }
}
