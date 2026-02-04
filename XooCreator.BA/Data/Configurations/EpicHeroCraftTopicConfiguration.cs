using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicHeroCraftTopicConfiguration : IEntityTypeConfiguration<EpicHeroCraftTopic>
{
    public void Configure(EntityTypeBuilder<EpicHeroCraftTopic> builder)
    {
        builder.ToTable("EpicHeroCraftTopic", "alchimalia_schema");
        builder.HasKey(x => new { x.EpicHeroCraftId, x.StoryTopicId });
        builder.HasOne(x => x.EpicHeroCraft).WithMany(x => x.Topics).HasForeignKey(x => x.EpicHeroCraftId);
        builder.HasOne(x => x.StoryTopic).WithMany().HasForeignKey(x => x.StoryTopicId);
    }
}
