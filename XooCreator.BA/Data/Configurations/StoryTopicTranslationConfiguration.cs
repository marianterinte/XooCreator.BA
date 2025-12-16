using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryTopicTranslationConfiguration : IEntityTypeConfiguration<StoryTopicTranslation>
{
    public void Configure(EntityTypeBuilder<StoryTopicTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.StoryTopicId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryTopic)
            .WithMany(t => t.Translations)
            .HasForeignKey(x => x.StoryTopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
