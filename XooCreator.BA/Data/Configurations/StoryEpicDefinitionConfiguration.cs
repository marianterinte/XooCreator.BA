using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicDefinitionConfiguration : IEntityTypeConfiguration<StoryEpicDefinition>
{
    public void Configure(EntityTypeBuilder<StoryEpicDefinition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.CoverImageUrl).HasMaxLength(500);
        builder.Property(x => x.MarketplaceCoverImageUrl).HasMaxLength(500);
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => new { x.OwnerUserId, x.Id }).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.AudioLanguages)
            .HasConversion(
                v => v == null || v.Count == 0 ? Array.Empty<string>() : v.ToArray(),
                v => v == null ? new List<string>() : v.ToList())
            .HasColumnType("text[]");
    }
}
