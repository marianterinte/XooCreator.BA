using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicHeroCraftRegionConfiguration : IEntityTypeConfiguration<EpicHeroCraftRegion>
{
    public void Configure(EntityTypeBuilder<EpicHeroCraftRegion> builder)
    {
        builder.ToTable("EpicHeroCraftRegion", "alchimalia_schema");
        builder.HasKey(x => new { x.EpicHeroCraftId, x.RegionId });
        builder.HasOne(x => x.EpicHeroCraft).WithMany(x => x.Regions).HasForeignKey(x => x.EpicHeroCraftId);
        builder.HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionId);
    }
}
