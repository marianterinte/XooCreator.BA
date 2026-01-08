using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryLikeConfiguration : IEntityTypeConfiguration<StoryLike>
{
    public void Configure(EntityTypeBuilder<StoryLike> builder)
    {
        builder.ToTable("StoryLikes", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LikedAt).IsRequired();
        builder.HasIndex(x => x.StoryId).HasDatabaseName("IX_StoryLikes_StoryId");
        builder.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique().HasDatabaseName("IX_StoryLikes_UserId_StoryId");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

