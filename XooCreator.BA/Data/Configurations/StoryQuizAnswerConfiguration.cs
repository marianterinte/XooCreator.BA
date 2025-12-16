using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryQuizAnswerConfiguration : IEntityTypeConfiguration<StoryQuizAnswer>
{
    public void Configure(EntityTypeBuilder<StoryQuizAnswer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TileId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SelectedAnswerId).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.StoryId, x.TileId, x.SessionId });
        builder.HasIndex(x => new { x.UserId, x.StoryId });
        builder.HasIndex(x => x.SessionId);
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
