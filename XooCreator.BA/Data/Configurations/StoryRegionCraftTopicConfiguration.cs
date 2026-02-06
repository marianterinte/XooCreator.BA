using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryRegionCraftTopicConfiguration : IEntityTypeConfiguration<StoryRegionCraftTopic>
{
    public void Configure(EntityTypeBuilder<StoryRegionCraftTopic> builder)
    {
        builder.ToTable("StoryRegionCraftTopic", "alchimalia_schema");
        builder.HasKey(x => new { x.StoryRegionCraftId, x.StoryTopicId });
        builder.HasOne(x => x.StoryRegionCraft).WithMany(x => x.Topics).HasForeignKey(x => x.StoryRegionCraftId);
        builder.HasOne(x => x.StoryTopic).WithMany().HasForeignKey(x => x.StoryTopicId);
    }
}
