using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryPurchaseConfiguration : IEntityTypeConfiguration<StoryPurchase>
{
    public void Configure(EntityTypeBuilder<StoryPurchase> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.UserId, x.StoryId }).IsUnique();
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Story).WithMany().HasForeignKey(x => x.StoryId).HasPrincipalKey(s => s.StoryId).OnDelete(DeleteBehavior.Cascade);
    }
}
