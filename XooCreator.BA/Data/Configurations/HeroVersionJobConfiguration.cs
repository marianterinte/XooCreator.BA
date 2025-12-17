using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroVersionJobConfiguration : IEntityTypeConfiguration<HeroVersionJob>
{
    public void Configure(EntityTypeBuilder<HeroVersionJob> builder)
    {
        builder.ToTable("HeroVersionJobs", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.HeroId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RequestedByEmail).HasMaxLength(256);
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);
        builder.HasIndex(x => new { x.HeroId, x.Status });
        builder.HasIndex(x => x.QueuedAtUtc);
    }
}

