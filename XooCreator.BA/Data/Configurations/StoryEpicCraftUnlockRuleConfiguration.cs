using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicCraftUnlockRuleConfiguration : IEntityTypeConfiguration<StoryEpicCraftUnlockRule>
{
    public void Configure(EntityTypeBuilder<StoryEpicCraftUnlockRule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
        builder.Property(x => x.FromId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ToRegionId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ToStoryId).HasMaxLength(100);
        builder.HasOne(x => x.Epic).WithMany(x => x.UnlockRules).HasForeignKey(x => x.EpicId).OnDelete(DeleteBehavior.Cascade);
    }
}
