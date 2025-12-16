using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftTileTranslationConfiguration : IEntityTypeConfiguration<StoryCraftTileTranslation>
{
    public void Configure(EntityTypeBuilder<StoryCraftTileTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.StoryCraftTileId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryCraftTile)
            .WithMany(t => t.Translations)
            .HasForeignKey(x => x.StoryCraftTileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
