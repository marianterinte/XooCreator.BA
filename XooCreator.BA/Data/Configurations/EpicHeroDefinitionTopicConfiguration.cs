using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicHeroDefinitionTopicConfiguration : IEntityTypeConfiguration<EpicHeroDefinitionTopic>
{
    public void Configure(EntityTypeBuilder<EpicHeroDefinitionTopic> builder)
    {
        builder.ToTable("EpicHeroDefinitionTopic", "alchimalia_schema");
        builder.HasKey(x => new { x.EpicHeroDefinitionId, x.StoryTopicId });
        builder.HasOne(x => x.EpicHeroDefinition).WithMany(x => x.Topics).HasForeignKey(x => x.EpicHeroDefinitionId);
        builder.HasOne(x => x.StoryTopic).WithMany().HasForeignKey(x => x.StoryTopicId);
    }
}
