using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class GenerativeLoiJobConfiguration : IEntityTypeConfiguration<GenerativeLoiJob>
{
    public void Configure(EntityTypeBuilder<GenerativeLoiJob> builder)
    {
        builder.ToTable("GenerativeLoiJobs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Locale).HasMaxLength(16);
        builder.Property(x => x.CombinationJson).HasMaxLength(512);
        builder.Property(x => x.Status).HasMaxLength(32);
        builder.Property(x => x.ProgressMessage).HasMaxLength(512);
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);
        builder.Property(x => x.ResultName).HasMaxLength(128);
        builder.Property(x => x.ResultImageUrl).HasMaxLength(1024);
        builder.Property(x => x.ResultStoryText).HasMaxLength(10000);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.QueuedAtUtc);
    }
}
