using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryTileTranslationConfiguration : IEntityTypeConfiguration<StoryTileTranslation>
{
    public void Configure(EntityTypeBuilder<StoryTileTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.Property(x => x.ContentHash).HasMaxLength(128);
        builder.HasIndex(x => new { x.StoryTileId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryTile)
            .WithMany(t => t.Translations)
            .HasForeignKey(x => x.StoryTileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
