using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicImportJobConfiguration : IEntityTypeConfiguration<EpicImportJob>
{
    public void Configure(EntityTypeBuilder<EpicImportJob> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EpicId).HasMaxLength(256).IsRequired();
        builder.Property(x => x.OriginalEpicId).HasMaxLength(256);
        builder.Property(x => x.RequestedByEmail).HasMaxLength(256);
        builder.Property(x => x.Locale).HasMaxLength(16);
        builder.Property(x => x.ZipBlobPath).HasMaxLength(512).IsRequired();
        builder.Property(x => x.ZipFileName).HasMaxLength(256);
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.Property(x => x.ConflictStrategy).HasMaxLength(16).IsRequired();
        builder.Property(x => x.IdPrefix).HasMaxLength(32);
        builder.Property(x => x.PhasesJson).HasColumnType("jsonb");
        builder.Property(x => x.IdMappingsJson).HasColumnType("jsonb");
        builder.Property(x => x.WarningsJson).HasColumnType("jsonb");
        builder.HasIndex(x => new { x.EpicId, x.Status });
        builder.HasIndex(x => x.OwnerUserId);
        builder.HasIndex(x => x.QueuedAtUtc);
    }
}