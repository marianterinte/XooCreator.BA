using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicAssetLinkConfiguration : IEntityTypeConfiguration<EpicAssetLink>
{
    public void Configure(EntityTypeBuilder<EpicAssetLink> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.Property(x => x.AssetType).HasMaxLength(32).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(200);
        builder.Property(x => x.DraftPath).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.PublishedPath).HasMaxLength(1024);
        builder.Property(x => x.ContentHash).HasMaxLength(128);
        builder.HasIndex(x => new { x.EpicId, x.DraftVersion });
        builder.HasIndex(x => x.DraftPath).IsUnique();
    }
}

