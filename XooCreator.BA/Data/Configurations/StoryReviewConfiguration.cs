using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryReviewConfiguration : IEntityTypeConfiguration<StoryReview>
{
    public void Configure(EntityTypeBuilder<StoryReview> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
        builder.Property(x => x.Rating).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(2000);
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Story).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId).OnDelete(DeleteBehavior.Cascade);
    }
}
