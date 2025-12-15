using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class EpicHeroTranslationConfiguration : IEntityTypeConfiguration<EpicHeroTranslation>
{
    public void Configure(EntityTypeBuilder<EpicHeroTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.EpicHeroId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.GreetingText).HasMaxLength(1000);
        builder.HasIndex(x => new { x.EpicHeroId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.EpicHero).WithMany(x => x.Translations).HasForeignKey(x => x.EpicHeroId).OnDelete(DeleteBehavior.Cascade);
    }
}
