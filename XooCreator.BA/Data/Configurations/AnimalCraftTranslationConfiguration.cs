using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalCraftTranslationConfiguration : IEntityTypeConfiguration<AnimalCraftTranslation>
{
    public void Configure(EntityTypeBuilder<AnimalCraftTranslation> builder)
    {
        builder.ToTable("AnimalCraftTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Label).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.AudioUrl).HasMaxLength(500);
        builder.HasIndex(x => new { x.AnimalCraftId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.AnimalCraftId);
        builder.HasOne(x => x.AnimalCraft).WithMany(x => x.Translations).HasForeignKey(x => x.AnimalCraftId).OnDelete(DeleteBehavior.Cascade);
    }
}
