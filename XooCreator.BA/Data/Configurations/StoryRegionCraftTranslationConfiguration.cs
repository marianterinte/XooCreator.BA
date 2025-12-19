using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryRegionCraftTranslationConfiguration : IEntityTypeConfiguration<StoryRegionCraftTranslation>
{
    public void Configure(EntityTypeBuilder<StoryRegionCraftTranslation> builder)
    {
        builder.ToTable("StoryRegionCraftTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryRegionCraftId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description);
        builder.HasIndex(x => new { x.StoryRegionCraftId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.StoryRegionCraftId);
        builder.HasOne(x => x.StoryRegionCraft).WithMany(x => x.Translations).HasForeignKey(x => x.StoryRegionCraftId).OnDelete(DeleteBehavior.Cascade);
    }
}

