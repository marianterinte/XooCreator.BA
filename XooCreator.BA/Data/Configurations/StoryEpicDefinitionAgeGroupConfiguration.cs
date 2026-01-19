using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicDefinitionAgeGroupConfiguration : IEntityTypeConfiguration<StoryEpicDefinitionAgeGroup>
{
    public void Configure(EntityTypeBuilder<StoryEpicDefinitionAgeGroup> builder)
    {
        builder.HasKey(x => new { x.StoryEpicDefinitionId, x.StoryAgeGroupId });
        builder.HasOne(x => x.StoryEpicDefinition).WithMany(x => x.AgeGroups).HasForeignKey(x => x.StoryEpicDefinitionId);
        builder.HasOne(x => x.StoryAgeGroup).WithMany().HasForeignKey(x => x.StoryAgeGroupId);
    }
}
