using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class UserOwnedStoriesConfiguration : IEntityTypeConfiguration<UserOwnedStories>
{
    public void Configure(EntityTypeBuilder<UserOwnedStories> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.UserId, x.StoryDefinitionId }).IsUnique();
        builder.Property(x => x.PurchasePrice).HasColumnType("decimal(10,2)");
        builder.Property(x => x.PurchaseReference).HasMaxLength(100);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.StoryDefinition).WithMany().HasForeignKey(x => x.StoryDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}
