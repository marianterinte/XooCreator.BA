using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class RegionVersionJobConfiguration : IEntityTypeConfiguration<RegionVersionJob>
{
    public void Configure(EntityTypeBuilder<RegionVersionJob> builder)
    {
        builder.ToTable("RegionVersionJobs", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.RegionId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RequestedByEmail).HasMaxLength(256);
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);
        builder.HasIndex(x => new { x.RegionId, x.Status });
        builder.HasIndex(x => x.QueuedAtUtc);
    }
}

