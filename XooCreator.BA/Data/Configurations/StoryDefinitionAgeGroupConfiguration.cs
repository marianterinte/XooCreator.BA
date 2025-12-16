using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryDefinitionAgeGroupConfiguration : IEntityTypeConfiguration<StoryDefinitionAgeGroup>
{
    public void Configure(EntityTypeBuilder<StoryDefinitionAgeGroup> builder)
    {
        builder.HasKey(x => new { x.StoryDefinitionId, x.StoryAgeGroupId });
        builder.HasOne(x => x.StoryDefinition).WithMany(x => x.AgeGroups).HasForeignKey(x => x.StoryDefinitionId);
        builder.HasOne(x => x.StoryAgeGroup).WithMany(x => x.StoryDefinitions).HasForeignKey(x => x.StoryAgeGroupId);
    }
}
