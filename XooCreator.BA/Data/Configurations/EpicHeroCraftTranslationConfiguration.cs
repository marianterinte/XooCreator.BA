using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicHeroCraftTranslationConfiguration : IEntityTypeConfiguration<EpicHeroCraftTranslation>
{
    public void Configure(EntityTypeBuilder<EpicHeroCraftTranslation> builder)
    {
        builder.ToTable("EpicHeroCraftTranslations", "alchimalia_schema");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicHeroCraftId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description);
        builder.Property(x => x.GreetingText).HasMaxLength(1000);
        builder.Property(x => x.GreetingAudioUrl).HasMaxLength(1000);
        builder.HasIndex(x => new { x.EpicHeroCraftId, x.LanguageCode }).IsUnique();
        builder.HasIndex(x => x.EpicHeroCraftId);
        builder.HasOne(x => x.EpicHeroCraft).WithMany(x => x.Translations).HasForeignKey(x => x.EpicHeroCraftId).OnDelete(DeleteBehavior.Cascade);
    }
}

