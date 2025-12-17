using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicHeroDefinitionTranslationConfiguration : IEntityTypeConfiguration<EpicHeroDefinitionTranslation>
{
    public void Configure(EntityTypeBuilder<EpicHeroDefinitionTranslation> builder)
    {
        builder.ToTable("EpicHeroDefinitionTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicHeroDefinitionId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description);
        builder.Property(x => x.GreetingText).HasMaxLength(1000);
        builder.Property(x => x.GreetingAudioUrl).HasMaxLength(1000);
        builder.HasIndex(x => new { x.EpicHeroDefinitionId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.EpicHeroDefinitionId);
        builder.HasOne(x => x.EpicHeroDefinition).WithMany(x => x.Translations).HasForeignKey(x => x.EpicHeroDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}

