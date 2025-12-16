using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftTranslationConfiguration : IEntityTypeConfiguration<StoryCraftTranslation>
{
    public void Configure(EntityTypeBuilder<StoryCraftTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.StoryCraftId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryCraft)
            .WithMany(s => s.Translations)
            .HasForeignKey(x => x.StoryCraftId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
