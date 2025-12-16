using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryHeroUnlockConfiguration : IEntityTypeConfiguration<StoryHeroUnlock>
{
    public void Configure(EntityTypeBuilder<StoryHeroUnlock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.StoryHeroId, x.StoryId }).IsUnique();
        builder.HasOne(x => x.StoryHero).WithMany(x => x.StoryUnlocks).HasForeignKey(x => x.StoryHeroId);
    }
}
