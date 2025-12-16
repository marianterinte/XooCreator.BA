using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data.Configurations;

public class StoryCraftAnswerTranslationConfiguration : IEntityTypeConfiguration<StoryCraftAnswerTranslation>
{
    public void Configure(EntityTypeBuilder<StoryCraftAnswerTranslation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => new { x.StoryCraftAnswerId, x.LanguageCode }).IsUnique();
        builder.HasOne(x => x.StoryCraftAnswer)
            .WithMany(a => a.Translations)
            .HasForeignKey(x => x.StoryCraftAnswerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
