using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicHeroDefinitionRegionConfiguration : IEntityTypeConfiguration<EpicHeroDefinitionRegion>
{
    public void Configure(EntityTypeBuilder<EpicHeroDefinitionRegion> builder)
    {
        builder.ToTable("EpicHeroDefinitionRegion", "alchimalia_schema");
        builder.HasKey(x => new { x.EpicHeroDefinitionId, x.RegionId });
        builder.HasOne(x => x.EpicHeroDefinition).WithMany(x => x.Regions).HasForeignKey(x => x.EpicHeroDefinitionId);
        builder.HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionId);
    }
}
