using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryDefinitionConfiguration : IEntityTypeConfiguration<StoryDefinition>
{
    public void Configure(EntityTypeBuilder<StoryDefinition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.StoryId).IsUnique();
        builder.Property(x => x.LastPublishedVersion).HasDefaultValue(0);
        builder.HasMany(x => x.Tiles).WithOne(x => x.StoryDefinition).HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Topics).WithOne(x => x.StoryDefinition).HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.AgeGroups).WithOne(x => x.StoryDefinition).HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ClassicAuthor)
            .WithMany(x => x.StoryDefinitions)
            .HasForeignKey(x => x.ClassicAuthorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
