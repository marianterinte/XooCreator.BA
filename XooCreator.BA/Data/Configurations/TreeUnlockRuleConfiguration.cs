using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class TreeUnlockRuleConfiguration : IEntityTypeConfiguration<TreeUnlockRule>
{
    public void Configure(EntityTypeBuilder<TreeUnlockRule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
        builder.Property(x => x.FromId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ToRegionId).HasMaxLength(50).IsRequired();
        builder.HasOne(x => x.TreeConfiguration).WithMany().HasForeignKey(x => x.TreeConfigurationId);
    }
}
