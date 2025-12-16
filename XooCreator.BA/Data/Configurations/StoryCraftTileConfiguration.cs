using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftTileConfiguration : IEntityTypeConfiguration<StoryCraftTile>
{
    public void Configure(EntityTypeBuilder<StoryCraftTile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryCraftId, x.TileId }).IsUnique();
        builder.HasOne(x => x.StoryCraft).WithMany(x => x.Tiles).HasForeignKey(x => x.StoryCraftId);
        builder.HasMany(x => x.Answers).WithOne(x => x.StoryCraftTile).HasForeignKey(x => x.StoryCraftTileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Translations).WithOne(x => x.StoryCraftTile).HasForeignKey(x => x.StoryCraftTileId).OnDelete(DeleteBehavior.Cascade);
    }
}
