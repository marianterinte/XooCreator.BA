using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftAnswerConfiguration : IEntityTypeConfiguration<StoryCraftAnswer>
{
    public void Configure(EntityTypeBuilder<StoryCraftAnswer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryCraftTileId, x.AnswerId }).IsUnique();
        builder.HasOne(x => x.StoryCraftTile).WithMany(x => x.Answers).HasForeignKey(x => x.StoryCraftTileId);
        builder.HasMany(x => x.Tokens).WithOne(x => x.StoryCraftAnswer).HasForeignKey(x => x.StoryCraftAnswerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Translations).WithOne(x => x.StoryCraftAnswer).HasForeignKey(x => x.StoryCraftAnswerId).OnDelete(DeleteBehavior.Cascade);
    }
}
