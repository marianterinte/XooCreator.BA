using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryAgeGroupConfiguration : IEntityTypeConfiguration<StoryAgeGroup>
{
    public void Configure(EntityTypeBuilder<StoryAgeGroup> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.AgeGroupId).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.AgeGroupId).IsUnique();
        builder.HasMany(x => x.Translations).WithOne(x => x.StoryAgeGroup).HasForeignKey(x => x.StoryAgeGroupId).OnDelete(DeleteBehavior.Cascade);
    }
}
