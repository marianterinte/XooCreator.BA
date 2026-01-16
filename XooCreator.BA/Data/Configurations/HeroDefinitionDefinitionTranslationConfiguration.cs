using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroDefinitionDefinitionTranslationConfiguration : IEntityTypeConfiguration<HeroDefinitionDefinitionTranslation>
{
    public void Configure(EntityTypeBuilder<HeroDefinitionDefinitionTranslation> builder)
    {
        builder.ToTable("HeroDefinitionDefinitionTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.HeroDefinitionDefinitionId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
        builder.Property(x => x.AudioUrl).HasMaxLength(500);
        builder.HasIndex(x => new { x.HeroDefinitionDefinitionId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.HeroDefinitionDefinitionId);
        builder.HasOne(x => x.HeroDefinitionDefinition).WithMany(x => x.Translations).HasForeignKey(x => x.HeroDefinitionDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}
