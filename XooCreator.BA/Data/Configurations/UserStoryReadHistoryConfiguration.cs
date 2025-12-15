using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserStoryReadHistoryConfiguration : IEntityTypeConfiguration<UserStoryReadHistory>
{
    public void Configure(EntityTypeBuilder<UserStoryReadHistory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        // Unique constraint: one history record per user per story
        builder.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
        builder.HasIndex(x => x.CompletedAt);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
