using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryAnswerConfiguration : IEntityTypeConfiguration<StoryAnswer>
{
    public void Configure(EntityTypeBuilder<StoryAnswer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.StoryTileId, x.AnswerId }).IsUnique();
        builder.HasOne(x => x.StoryTile).WithMany(x => x.Answers).HasForeignKey(x => x.StoryTileId);
        builder.HasMany(x => x.Tokens).WithOne(x => x.StoryAnswer).HasForeignKey(x => x.StoryAnswerId).OnDelete(DeleteBehavior.Cascade);
    }
}
