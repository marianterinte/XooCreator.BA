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
        builder.HasOne(x => x.Region).WithMany(x => x.Animals).HasForeignKey(x => x.RegionId);
    }
}
