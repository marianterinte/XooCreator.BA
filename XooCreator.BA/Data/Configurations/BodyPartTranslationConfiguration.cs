using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class BodyPartTranslationConfiguration : IEntityTypeConfiguration<BodyPartTranslation>
{
    public void Configure(EntityTypeBuilder<BodyPartTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.BodyPartKey, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.BodyPart)
            .WithMany(b => b.Translations)
            .HasForeignKey(x => x.BodyPartKey)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
