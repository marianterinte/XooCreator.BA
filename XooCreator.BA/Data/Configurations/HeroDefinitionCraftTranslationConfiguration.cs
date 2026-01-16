using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroDefinitionCraftTranslationConfiguration : IEntityTypeConfiguration<HeroDefinitionCraftTranslation>
{
    public void Configure(EntityTypeBuilder<HeroDefinitionCraftTranslation> builder)
    {
        builder.ToTable("HeroDefinitionCraftTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.HeroDefinitionCraftId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
        builder.Property(x => x.AudioUrl).HasMaxLength(500);
        builder.HasIndex(x => new { x.HeroDefinitionCraftId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.HeroDefinitionCraftId);
        builder.HasOne(x => x.HeroDefinitionCraft).WithMany(x => x.Translations).HasForeignKey(x => x.HeroDefinitionCraftId).OnDelete(DeleteBehavior.Cascade);
    }
}
