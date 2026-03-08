using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryAIGenerateJobConfiguration : IEntityTypeConfiguration<StoryAIGenerateJob>
{
    public void Configure(EntityTypeBuilder<StoryAIGenerateJob> builder)
    {
        builder.ToTable("StoryAIGenerateJobs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.RequestedByEmail).HasMaxLength(255).IsRequired();
        builder.Property(x => x.OwnerFirstName).HasMaxLength(255);
        builder.Property(x => x.OwnerLastName).HasMaxLength(255);
        builder.Property(x => x.Locale).HasMaxLength(16);
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ErrorCode).HasMaxLength(100);
        builder.Property(x => x.StoryId).HasMaxLength(255);
        builder.Property(x => x.RequestJson).IsRequired();
        builder.HasIndex(x => x.OwnerUserId);
        builder.HasIndex(x => new { x.Status, x.QueuedAtUtc });
    }
}
