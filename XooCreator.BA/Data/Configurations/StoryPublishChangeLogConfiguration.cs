using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryPublishChangeLogConfiguration : IEntityTypeConfiguration<StoryPublishChangeLog>
{
    public void Configure(EntityTypeBuilder<StoryPublishChangeLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(32).IsRequired();
        builder.Property(x => x.ChangeType).HasMaxLength(32).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(200);
        builder.Property(x => x.Hash).HasMaxLength(128);
        builder.Property(x => x.AssetDraftPath).HasMaxLength(1024);
        builder.Property(x => x.AssetPublishedPath).HasMaxLength(1024);
        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");
        builder.HasIndex(x => new { x.StoryId, x.DraftVersion });
    }
}
