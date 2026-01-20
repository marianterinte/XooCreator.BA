using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalDefinitionTranslationConfiguration : IEntityTypeConfiguration<AnimalDefinitionTranslation>
{
    public void Configure(EntityTypeBuilder<AnimalDefinitionTranslation> builder)
    {
        builder.ToTable("AnimalDefinitionTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Label).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.AudioUrl).HasMaxLength(500);
        builder.HasIndex(x => new { x.AnimalDefinitionId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.AnimalDefinitionId);
        builder.HasOne(x => x.AnimalDefinition).WithMany(x => x.Translations).HasForeignKey(x => x.AnimalDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}
