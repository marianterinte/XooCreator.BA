using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserFavoriteStoriesConfiguration : IEntityTypeConfiguration<UserFavoriteStories>
{
    public void Configure(EntityTypeBuilder<UserFavoriteStories> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.AddedAt).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.StoryDefinitionId }).IsUnique();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.StoryDefinition)
            .WithMany()
            .HasForeignKey(x => x.StoryDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
