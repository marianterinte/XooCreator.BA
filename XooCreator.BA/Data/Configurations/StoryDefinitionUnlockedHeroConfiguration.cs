using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Configurations;

public class StoryDefinitionUnlockedHeroConfiguration : IEntityTypeConfiguration<StoryDefinitionUnlockedHero>
{
    public void Configure(EntityTypeBuilder<StoryDefinitionUnlockedHero> builder)
    {
        builder.HasKey(x => new { x.StoryDefinitionId, x.HeroId });
        builder.HasOne(x => x.StoryDefinition)
            .WithMany(x => x.UnlockedHeroes)
            .HasForeignKey(x => x.StoryDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.HeroId)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
