using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicCraftCoAuthorConfiguration : IEntityTypeConfiguration<StoryEpicCraftCoAuthor>
{
    public void Configure(EntityTypeBuilder<StoryEpicCraftCoAuthor> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryEpicCraftId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(500);
        builder.Property(x => x.SortOrder).HasDefaultValue(0);
        builder.HasOne(x => x.StoryEpicCraft).WithMany(x => x.CoAuthors).HasForeignKey(x => x.StoryEpicCraftId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => x.StoryEpicCraftId);
        builder.HasIndex(x => x.UserId).HasFilter("\"UserId\" IS NOT NULL");
    }
}
