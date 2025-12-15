using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicStoryProgressConfiguration : IEntityTypeConfiguration<EpicStoryProgress>
{
    public void Configure(EntityTypeBuilder<EpicStoryProgress> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.UserId, x.StoryId, x.EpicId }).IsUnique();
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Epic).WithMany().HasForeignKey(x => x.EpicId).OnDelete(DeleteBehavior.Cascade);
    }
}
