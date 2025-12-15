using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class TreeRegionConfiguration : IEntityTypeConfiguration<TreeRegion>
{
    public void Configure(EntityTypeBuilder<TreeRegion> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.Label).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.Id, x.TreeConfigurationId }).IsUnique();
        builder.HasOne(x => x.TreeConfiguration).WithMany().HasForeignKey(x => x.TreeConfigurationId);
    }
}
