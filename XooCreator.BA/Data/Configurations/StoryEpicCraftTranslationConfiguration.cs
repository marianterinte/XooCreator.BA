using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryEpicCraftTranslationConfiguration : IEntityTypeConfiguration<StoryEpicCraftTranslation>
{
    public void Configure(EntityTypeBuilder<StoryEpicCraftTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.StoryEpicCraftId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => new { x.StoryEpicCraftId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryEpicCraft).WithMany(x => x.Translations).HasForeignKey(x => x.StoryEpicCraftId).OnDelete(DeleteBehavior.Cascade);
    }
}
