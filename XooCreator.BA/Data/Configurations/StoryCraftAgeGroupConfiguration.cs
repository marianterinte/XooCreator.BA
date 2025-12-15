using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftAgeGroupConfiguration : IEntityTypeConfiguration<StoryCraftAgeGroup>
{
    public void Configure(EntityTypeBuilder<StoryCraftAgeGroup> builder)
    {
        builder.HasKey(x => new { x.StoryCraftId, x.StoryAgeGroupId });
        builder.HasOne(x => x.StoryCraft).WithMany(x => x.AgeGroups).HasForeignKey(x => x.StoryCraftId);
        builder.HasOne(x => x.StoryAgeGroup).WithMany(x => x.StoryCrafts).HasForeignKey(x => x.StoryAgeGroupId);
    }
}
