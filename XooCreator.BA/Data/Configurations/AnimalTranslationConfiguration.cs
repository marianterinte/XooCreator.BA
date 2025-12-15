using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class AnimalTranslationConfiguration : IEntityTypeConfiguration<AnimalTranslation>
{
    public void Configure(EntityTypeBuilder<AnimalTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.AnimalId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.Animal)
            .WithMany(a => a.Translations)
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
