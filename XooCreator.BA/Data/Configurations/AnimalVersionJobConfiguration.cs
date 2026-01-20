using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalVersionJobConfiguration : IEntityTypeConfiguration<AnimalVersionJob>
{
    public void Configure(EntityTypeBuilder<AnimalVersionJob> builder)
    {
        builder.ToTable("AnimalVersionJobs", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.RequestedByEmail).HasMaxLength(256);
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);
        builder.HasIndex(x => new { x.AnimalId, x.Status });
        builder.HasIndex(x => x.QueuedAtUtc);
        builder.HasOne<AlchimaliaUser>().WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
