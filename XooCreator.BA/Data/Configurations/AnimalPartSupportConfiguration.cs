using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalPartSupportConfiguration : IEntityTypeConfiguration<AnimalPartSupport>
{
    public void Configure(EntityTypeBuilder<AnimalPartSupport> builder)
    {
        builder.HasKey(x => new { x.AnimalId, x.PartKey });
        builder.HasOne(x => x.Animal).WithMany(x => x.SupportedParts).HasForeignKey(x => x.AnimalId);
        builder.HasOne(x => x.Part).WithMany().HasForeignKey(x => x.PartKey);
    }
}
