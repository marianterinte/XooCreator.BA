using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryForkJobConfiguration : IEntityTypeConfiguration<StoryForkJob>
{
    public void Configure(EntityTypeBuilder<StoryForkJob> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SourceStoryId).HasMaxLength(256).IsRequired();
        builder.Property(x => x.SourceType).HasMaxLength(32).IsRequired();
        builder.Property(x => x.TargetStoryId).HasMaxLength(256).IsRequired();
        builder.Property(x => x.TargetOwnerEmail).HasMaxLength(256).IsRequired();
        builder.Property(x => x.RequestedByEmail).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => new { x.TargetStoryId, x.Status });
    }
}
