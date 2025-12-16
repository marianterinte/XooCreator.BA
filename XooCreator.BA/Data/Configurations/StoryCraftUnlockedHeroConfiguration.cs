using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftUnlockedHeroConfiguration : IEntityTypeConfiguration<StoryCraftUnlockedHero>
{
    public void Configure(EntityTypeBuilder<StoryCraftUnlockedHero> builder)
    {
        builder.HasKey(x => new { x.StoryCraftId, x.HeroId });
        builder.HasOne(x => x.StoryCraft)
            .WithMany(x => x.UnlockedHeroes)
            .HasForeignKey(x => x.StoryCraftId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.HeroId)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}

