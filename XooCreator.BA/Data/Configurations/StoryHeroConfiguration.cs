using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryHeroConfiguration : IEntityTypeConfiguration<StoryHero>
{
    public void Configure(EntityTypeBuilder<StoryHero> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.HeroId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.UnlockConditionJson).HasMaxLength(2000);
        builder.HasIndex(x => x.HeroId).IsUnique();
        builder.HasMany(x => x.StoryUnlocks).WithOne(x => x.StoryHero).HasForeignKey(x => x.StoryHeroId).OnDelete(DeleteBehavior.Cascade);
    }
}
