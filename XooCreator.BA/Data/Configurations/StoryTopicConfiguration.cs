using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryTopicConfiguration : IEntityTypeConfiguration<StoryTopic>
{
    public void Configure(EntityTypeBuilder<StoryTopic> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.TopicId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.DimensionId).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.TopicId).IsUnique();
        builder.HasMany(x => x.Translations).WithOne(x => x.StoryTopic).HasForeignKey(x => x.StoryTopicId).OnDelete(DeleteBehavior.Cascade);
    }
}
