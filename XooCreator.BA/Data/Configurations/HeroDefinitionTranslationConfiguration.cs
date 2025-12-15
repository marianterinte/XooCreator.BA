using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class HeroDefinitionTranslationConfiguration : IEntityTypeConfiguration<HeroDefinitionTranslation>
{
    public void Configure(EntityTypeBuilder<HeroDefinitionTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.HeroDefinitionId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.HeroDefinition)
            .WithMany(s => s.Translations)
            .HasForeignKey(x => x.HeroDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
