using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryImportJobConfiguration : IEntityTypeConfiguration<StoryImportJob>
{
    public void Configure(EntityTypeBuilder<StoryImportJob> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.StoryId).HasMaxLength(256).IsRequired();
        builder.Property(x => x.OriginalStoryId).HasMaxLength(256);
        builder.Property(x => x.Locale).HasMaxLength(16);
        builder.Property(x => x.ZipBlobPath).HasMaxLength(512);
        builder.Property(x => x.StagingPrefix).HasMaxLength(512);
        builder.Property(x => x.ManifestBlobPath).HasMaxLength(512);
        builder.Property(x => x.ZipFileName).HasMaxLength(256);
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => new { x.StoryId, x.Status });
        builder.HasIndex(x => x.OwnerUserId);
    }
}
