using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserCreatedStoriesConfiguration : IEntityTypeConfiguration<UserCreatedStories>
{
    public void Configure(EntityTypeBuilder<UserCreatedStories> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.UserId, x.StoryDefinitionId }).IsUnique();
        builder.Property(x => x.CreationNotes).HasMaxLength(1000);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.StoryDefinition).WithMany().HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}
