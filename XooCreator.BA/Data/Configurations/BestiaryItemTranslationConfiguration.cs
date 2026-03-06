using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XooCreator.BA.Data.Configurations;

public class BestiaryItemTranslationConfiguration : IEntityTypeConfiguration<BestiaryItemTranslation>
{
    public void Configure(EntityTypeBuilder<BestiaryItemTranslation> builder)
    {
        builder.ToTable("BestiaryItemTranslations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.Property(x => x.Name).HasMaxLength(256);
        builder.Property(x => x.Story).HasMaxLength(10000);
        builder.HasIndex(x => new { x.BestiaryItemId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.BestiaryItem)
            .WithMany(b => b.Translations)
            .HasForeignKey(x => x.BestiaryItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
