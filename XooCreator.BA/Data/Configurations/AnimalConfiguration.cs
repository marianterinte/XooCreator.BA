using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Label).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Src).HasMaxLength(256).IsRequired();
        builder.Property(x => x.IsHybrid).HasDefaultValue(false);
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired().HasDefaultValue("draft");
        builder.Property(x => x.Version).IsRequired().HasDefaultValue(1);
        builder.Property(x => x.ReviewNotes).HasMaxLength(2000);
        builder.HasOne(x => x.Region).WithMany(x => x.Animals).HasForeignKey(x => x.RegionId).IsRequired(false);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.Id, x.Status });
    }
}
