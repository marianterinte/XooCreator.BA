using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicCraftAgeGroupConfiguration : IEntityTypeConfiguration<StoryEpicCraftAgeGroup>
{
    public void Configure(EntityTypeBuilder<StoryEpicCraftAgeGroup> builder)
    {
        builder.HasKey(x => new { x.StoryEpicCraftId, x.StoryAgeGroupId });
        builder.HasOne(x => x.StoryEpicCraft).WithMany(x => x.AgeGroups).HasForeignKey(x => x.StoryEpicCraftId);
        builder.HasOne(x => x.StoryAgeGroup).WithMany().HasForeignKey(x => x.StoryAgeGroupId);
    }
}
