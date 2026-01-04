using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicExportJobConfiguration : IEntityTypeConfiguration<EpicExportJob>
{
    public void Configure(EntityTypeBuilder<EpicExportJob> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EpicId).HasMaxLength(256).IsRequired();
        builder.Property(x => x.RequestedByEmail).HasMaxLength(256);
        builder.Property(x => x.Locale).HasMaxLength(16);
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.Property(x => x.CurrentPhase).HasMaxLength(32);
        builder.Property(x => x.ZipBlobPath).HasMaxLength(512);
        builder.Property(x => x.ZipFileName).HasMaxLength(256);
        builder.Property(x => x.LanguageFilter).HasMaxLength(16);
        builder.HasIndex(x => new { x.EpicId, x.Status });
        builder.HasIndex(x => x.OwnerUserId);
        builder.HasIndex(x => x.QueuedAtUtc);
    }
}