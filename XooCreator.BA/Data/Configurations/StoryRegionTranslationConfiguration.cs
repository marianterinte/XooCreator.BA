using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryRegionTranslationConfiguration : IEntityTypeConfiguration<StoryRegionTranslation>
{
    public void Configure(EntityTypeBuilder<StoryRegionTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryRegionId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.StoryRegionId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryRegion).WithMany(x => x.Translations).HasForeignKey(x => x.StoryRegionId).OnDelete(DeleteBehavior.Cascade);
    }
}
