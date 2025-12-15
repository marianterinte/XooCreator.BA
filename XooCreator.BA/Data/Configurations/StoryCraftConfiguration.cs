using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftConfiguration : IEntityTypeConfiguration<StoryCraft>
{
    public void Configure(EntityTypeBuilder<StoryCraft> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.LastDraftVersion).HasDefaultValue(0);
        builder.HasIndex(x => x.StoryId).IsUnique();
        builder.HasMany(x => x.Translations).WithOne(x => x.StoryCraft).HasForeignKey(x => x.StoryCraftId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Tiles).WithOne(x => x.StoryCraft).HasForeignKey(x => x.StoryCraftId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Topics).WithOne(x => x.StoryCraft).HasForeignKey(x => x.StoryCraftId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.AgeGroups).WithOne(x => x.StoryCraft).HasForeignKey(x => x.StoryCraftId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ClassicAuthor)
            .WithMany()
            .HasForeignKey(x => x.ClassicAuthorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
