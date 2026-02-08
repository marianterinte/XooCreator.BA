using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryTranslationJobConfiguration : IEntityTypeConfiguration<StoryTranslationJob>
{
    public void Configure(EntityTypeBuilder<StoryTranslationJob> builder)
    {
        builder.ToTable("StoryTranslationJobs", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.StoryId).HasMaxLength(255).IsRequired();
        builder.Property(x => x.ReferenceLanguage).HasMaxLength(10).IsRequired();
        builder.Property(x => x.TargetLanguagesJson).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.StoryId, x.Status });
        builder.HasIndex(x => x.QueuedAtUtc);
        builder.HasIndex(x => x.OwnerUserId);
    }
}
